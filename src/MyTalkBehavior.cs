using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Bannerlord.UIExtenderEx.Prefabs2;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.GameMenus;
using Helpers;

namespace MB2MultiCheats
{
    public static class MyTalkExtensions
    {
        // 哪些角色可以聊天
        public static bool CanTalkTo(this Hero hero)
        {
            // CharacterStates && hero.IsAlive && !hero.IsPrisoner && !mainHero.IsPrisoner
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

    internal class MyTalkBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, OnConversationEnded);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private static LinkedList<Messenger> Messengers = new LinkedList<Messenger>();

        private static PlayerEncounter meeting = null;

        private static Hero meetingHero = null;

        private static PlayerEncounter keepEncounter = null;

        private static LocationEncounter keepLocation = null;

        private static Settlement keepSettlement = null;

        private class Messenger
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

                meeting = null;
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
                return meeting != null;
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

        // 开始会面
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
                        if ((targetParty = player.HomeSettlement?.Party) == null)
                        {
                            foreach (Settlement settlement in Settlement.All)
                            {
                                if ((targetParty = settlement.Party) != null) break;
                            }
                        }
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
                meeting = PlayerEncounter.Current;
                meetingHero = target;

                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.CurrentConversationContext = ConversationContext.Default;
                AccessTools.Field(typeof(PlayerEncounter), "_mapEventState").SetValue(PlayerEncounter.Current, PlayerEncounterState.Begin);
                AccessTools.Field(typeof(PlayerEncounter), "_stateHandled").SetValue(PlayerEncounter.Current, true);
                AccessTools.Field(typeof(PlayerEncounter), "_meetingDone").SetValue(PlayerEncounter.Current, true);
            }

            MCLog.Info("信使会面: " + messenger.Hero.ToString() + " -> " + messenger.Hours.ToString() + "小时");
            CampaignMission.OpenConversationMission(new ConversationCharacterData(player.CharacterObject, playerParty), new ConversationCharacterData(target.CharacterObject, targetParty), "", "");
        }
    }

    // [PrefabExtension("EncyclopediaHeroPage", "descendant::TextWidget[@Text='@SkillsText']")]
    [PrefabExtension("EncyclopediaHeroPage", "descendant::RichTextWidget[@Text='@InformationText']")]
    internal class EncyclopediaHeroPagePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionText]
        public string GetContent => @"
            <Widget WidthSizePolicy=""CoverChildren"" HeightSizePolicy=""CoverChildren"" VerticalAlignment=""Top"" MarginTop=""20"" HorizontalAlignment=""Center"">
                <Children>
                    <ButtonWidget Command.Click=""CallToTalk"" IsVisible=""@CanTalkTo"" IsEnabled=""@WillNotTalk"" SuggestedWidth=""400"" SuggestedHeight=""40"" WidthSizePolicy=""Fixed"" HeightSizePolicy=""Fixed"" Brush=""ButtonBrush2"" DoNotPassEventsToChildren=""true"" UpdateChildrenStates=""true"">
                        <Children>
                            <TextWidget Text=""@CallToTalkText"" WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent"" Brush=""Kingdom.GeneralButtons.Text""/>
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

        public EncyclopediaHeroPageVMMixin(EncyclopediaHeroPageVM vm) : base(vm)
        {
            hero = vm.Obj as Hero;
            if (hero != null)
            {
                MCLog.Info(hero.Name.ToString());
            }
            // MCLog.Warn(vm.Master.Hero.Name + " -> " + hero.Name);
            // You will pay {AMOUNT}{GOLD_ICON} denars.
            
            CallToTalkText = new TextObject(hero.HasMet ? "发送信使(500第纳尔)" : "发送信使(2000第纳尔)", null).ToString();
            // CallToTalkText = new TextObject("发送信使(2000第纳尔)", null).ToString();
            // CallToTalkText = new TextObject("Send Messenger(500 denars)", null).ToString();
            // CallToTalkText = new TextObject("Send Messenger(2000 denars)", null).ToString();
        }

        [DataSourceMethod]
        public void CallToTalk()
        {
        }

        [DataSourceProperty]
        public string CallToTalkText { get; }

        [DataSourceProperty]
        public bool CanTalkTo
        {
            get
            {
                return true;
                // return hero != null && hero.CanTalkTo();
            }
        }

        [DataSourceProperty]
        public bool WillNotTalk
        {
            get
            {
                return true;
                // return !MyTalkBehavior.MessengerSended(hero);
            }
        }
    }
}

