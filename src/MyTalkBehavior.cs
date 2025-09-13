
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;

using HarmonyLib;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using Bannerlord.UIExtenderEx.Prefabs2;


namespace MB2MultiCheats
{
    // 百科英雄页面添加按钮组件
    [PrefabExtension("EncyclopediaHeroPage", "descendant::RichTextWidget[@Text='@InformationText']")]
    internal class EncyclopediaHeroPagePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionText]
        public string GetContent => @"
            <Widget WidthSizePolicy=""CoverChildren"" HeightSizePolicy=""CoverChildren"" VerticalAlignment=""Top"" MarginTop=""20"" HorizontalAlignment=""Center"">
                <Children>
                    <ButtonWidget Command.Click=""MCSendMessenger"" IsVisible=""@MCCanSendTo"" IsEnabled=""@MCCanSendNow"" SuggestedWidth=""400"" SuggestedHeight=""40"" WidthSizePolicy=""Fixed"" HeightSizePolicy=""Fixed"" Brush=""ButtonBrush2"" DoNotPassEventsToChildren=""true"" UpdateChildrenStates=""true"">
                        <Children>
                            <TextWidget Text=""@MCSendMessengerText"" WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent"" Brush=""Kingdom.GeneralButtons.Text""/>
                        </Children>
                    </ButtonWidget>
                </Children>
            </Widget>
        ";
    }

    [ViewModelMixin(nameof(EncyclopediaHeroPageVM.Refresh))]
    public class EncyclopediaHeroPageVMMixin : BaseViewModelMixin<EncyclopediaHeroPageVM>
    {
        private readonly Hero hero;
        private readonly int cost;

        public EncyclopediaHeroPageVMMixin(EncyclopediaHeroPageVM vm) : base(vm)
        {
            hero = vm.Obj as Hero;
            if (hero == null) return;
            cost = hero.HasMet ? 500 : 2000;

            MBTextManager.SetTextVariable("MC_Send_Messenger_Cost", cost.ToString());
            MCSendMessengerText = new TextObject("{=mcSendMessengerText}Send Messenger({MC_Send_Messenger_Cost} denars)").ToString();
        }

        [DataSourceProperty]
        public string MCSendMessengerText { get; }

        [DataSourceMethod]
        public void MCSendMessenger()
        {
            Hero.MainHero.GiveGoldToSettment(cost);
            MyTalkBehavior.SendMessenger(hero);
            OnPropertyChanged("MCCanSendNow");

            MBTextManager.SetTextVariable("MC_Send_Messenger_Hero", hero.Name.ToString());
            MCLog.Info("{=mcSendMessengerClick}The messenger has departed. You will soon meet {MC_Send_Messenger_Hero}...");
        }

        [DataSourceProperty]
        public bool MCCanSendTo
        {
            get
            {
                return hero != null && hero != Hero.MainHero && hero.IsAlive;
            }
        }

        [DataSourceProperty]
        public bool MCCanSendNow
        {
            get
            {
                return hero != null && hero != Hero.MainHero && hero.IsActive
                    && hero != MyTalkBehavior.MeetingHero
                    && !MyTalkBehavior.MessengerSended(hero);
            }
        }
    }

    internal static class MyTalkExtensions
    {
        // 是否可以开始聊天
        public static bool CanTalkTo(this Hero hero)
        {
            return hero != Hero.MainHero && hero.IsActive && Hero.MainHero.IsActive
                && Hero.MainHero.PartyBelongedTo != null
                && Hero.MainHero.PartyBelongedTo.MapEvent == null
                && hero.PartyBelongedTo?.MapEvent == null;
        }

        // 哪些对话需要禁用
        public static void BlockSentences(this CampaignGameStarter gameInitializer, Func<bool> condition, params string[] sentenceIds)
        {
            foreach (string sentenceId in sentenceIds)
            {
                ConversationSentence sentence = gameInitializer.GetSentence(sentenceId);
                if (sentence != null)
                {
                    ConversationSentence.OnConditionDelegate sentenceCondition = sentence.OnCondition;
                    sentence.OnCondition = (() => condition() && (sentenceCondition == null || sentenceCondition.Invoke()));
                }
            }
        }

        public static ConversationSentence GetSentence(this CampaignGameStarter gameInitializer, string id)
        {
            ConversationManager conversationManager = AccessTools.Field(typeof(CampaignGameStarter), "_conversationManager").GetValue(gameInitializer) as ConversationManager;
            return (AccessTools.Field(typeof(ConversationManager), "_sentences").GetValue(conversationManager) as List<ConversationSentence>).SingleOrDefault((ConversationSentence x) => x.Id.Equals(id));
        }

        // 获取角色当前位置
        public static Vec2 GetHeroPosition(this Hero hero)
        {
            return (hero.GetMapPoint()?.Position2D ?? hero.HomeSettlement?.Position2D) ?? Vec2.Zero;
        }

        // 获取角色距离
        public static float GetHeroDistance(this Hero baseHero, Hero targetHero)
        {
            return (baseHero.GetHeroPosition() - targetHero.GetHeroPosition()).Length;
        }

        // 在定居点消费
        public static void GiveGoldToSettment(this Hero hero, int amount)
        {
            hero.ChangeHeroGold(-amount);
            if (Settlement.CurrentSettlement?.SettlementComponent != null)
            {
                Settlement.CurrentSettlement.SettlementComponent.ChangeGold(amount);
            }
        }
    }

    internal class Messenger
    {
        public Hero Hero { get; protected set; }

        public bool Ready { get; protected set; }

        public int Hours { get; protected set; }

        private const float SpeedPerHour = 30f;

        public Messenger(Hero hero)
        {
            Hero = hero;
            Ready = false;
            Hours = 0;
        }

        public void HourlyTick()
        {
            Hours += 1;
            if (!Ready && Hero.MainHero.GetHeroDistance(Hero) <= SpeedPerHour * (float)Hours)
            {
                Ready = true;
            }
        }
    }

    internal class MyTalkBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, OnConversationEnded);
        }

        public override void SyncData(IDataStore dataStore) { }

        private static LinkedList<Messenger> Messengers = new LinkedList<Messenger>();

        private static Hero meetingHero = null;

        private static PlayerEncounter keepEncounter = null;

        private static LocationEncounter keepLocation = null;

        private static Settlement keepSettlement = null;

        private void OnGameLoaded(CampaignGameStarter game)
        {
            Messengers.Clear();
        }

        private void OnSessionLaunched(CampaignGameStarter game)
        {
            game.BlockSentences(() => !InMeeting, new string[] {
                "main_option_discussions_1",
                "hero_give_issue",
                "hero_task_given",
                "caravan_create_conversation_1",
                "main_option_discussions_2",
                "clan_member_manage_troops",
                "cheat_choice",
            });
        }

        private void OnHourlyTick()
        {
            foreach (Messenger msger in Messengers)
            {
                msger.HourlyTick();
            }

            Messenger messenger = Messengers.FirstOrDefault((Messenger x) => x.Ready);
            if (messenger != null && messenger.Hero.CanTalkTo())
            {
                Messengers.Remove(messenger);
                StartMeeting(messenger);
            }
        }

        private void OnConversationEnded(IEnumerable<CharacterObject> character)
        {
            if (InMeeting)
            {
                PlayerEncounter.Finish(false);
                AccessTools.Property(typeof(Campaign), "PlayerEncounter").SetValue(Campaign.Current, keepEncounter);
                AccessTools.Property(typeof(Campaign), "LocationEncounter").SetValue(Campaign.Current, keepLocation);
                Hero.MainHero.PartyBelongedTo.CurrentSettlement = keepSettlement;

                meetingHero = null;
                keepEncounter = null;
                keepLocation = null;
                keepSettlement = null;
            }
        }

        // 是否会话中
        public static bool InMeeting
        {
            get
            {
                return meetingHero != null;
            }
        }

        public static Hero MeetingHero
        {
            get
            {
                return meetingHero;
            }
        }

        // 是否已发送信使
        public static bool MessengerSended(Hero hero)
        {
            return Messengers.SingleOrDefault((Messenger x) => x.Hero == hero) != null;
        }

        // 发送信使
        public static void SendMessenger(Hero hero)
        {
            if (!MessengerSended(hero))
            {
                Messengers.AddLast(new Messenger(hero));
            }
        }

        // 开始会面 -> 应该还可以精简
        private void StartMeeting(Messenger messenger)
        {
            Hero player = Hero.MainHero;
            Hero target = messenger.Hero;

            PartyBase playerParty = player.PartyBelongedTo?.Party;
            PartyBase targetParty = target.PartyBelongedTo?.Party;

            // 没有团队，或者是玩家团队，则依次取 英雄定居点所属团队，英雄最近定居点所属团队，玩家定居点所属团队
            if (targetParty == null || playerParty == targetParty)
            {
                if ((targetParty = target.HomeSettlement?.Party) == null)
                {
                    if ((targetParty = target.LastKnownClosestSettlement?.Party) == null)
                    {
                        if ((targetParty = player.HomeSettlement?.Party) == null) { }
                    }
                }
            }

            // 有团队，或者不是流浪者
            if (targetParty != null || !target.IsWanderer)
            {
                keepEncounter = PlayerEncounter.Current;
                keepLocation = (LocationEncounter)AccessTools.Property(typeof(Campaign), "LocationEncounter").GetValue(Campaign.Current);
                keepSettlement = player.PartyBelongedTo.CurrentSettlement;

                // 不是流浪者，但已知定居点位置，则切换玩家当前英雄定居点
                if (targetParty == null && target.CurrentSettlement != null)
                {
                    player.PartyBelongedTo.CurrentSettlement = target.CurrentSettlement;
                }

                // 开始遭遇
                PlayerEncounter.Start();
                PlayerEncounter.Current.SetupFields(playerParty, targetParty ?? playerParty);
                meetingHero = target;

                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.CurrentConversationContext = ConversationContext.Default;
                AccessTools.Field(typeof(PlayerEncounter), "_mapEventState").SetValue(PlayerEncounter.Current, PlayerEncounterState.Begin);
                AccessTools.Field(typeof(PlayerEncounter), "_stateHandled").SetValue(PlayerEncounter.Current, true);
                AccessTools.Field(typeof(PlayerEncounter), "_meetingDone").SetValue(PlayerEncounter.Current, true);
            }
            CampaignMission.OpenConversationMission(new ConversationCharacterData(player.CharacterObject, playerParty), new ConversationCharacterData(target.CharacterObject, targetParty), "", "");
        }
    }

    // 解决信使会面崩溃问题 -> 尚不清楚原因
    [HarmonyPatch(typeof(FlattenedTroopRoster), "GenerateUniqueNoFromParty")]
    public class FlattenedTroopRosterPatch
    {
        public static bool Prefix(MobileParty party, int troopIndex, ref int __result)
        {
            __result = ((party?.Party?.Index).GetValueOrDefault(1) * 999983 + troopIndex * 100003) % 616841;
            return false;
        }
    }
}
