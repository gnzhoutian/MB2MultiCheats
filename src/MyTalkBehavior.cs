using HarmonyLib;
using MCM.Abstractions.Base.Global;

using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using Helpers;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Settlements;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.Actions;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

using JetBrains.Annotations;

using System.Xml;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;

namespace MB2MultiCheats
{
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

        private void OnGameLoaded(CampaignGameStarter game)
        {
            Messengers.Clear();
        }

        private void OnSessionLaunched(CampaignGameStarter game)
        {
            game.BlockSentences(() => !MeetingInProgress, new string[]
            {
                "main_option_discussions_1",
                "hero_give_issue",
                "hero_task_given",
                "caravan_create_conversation_1",
                "main_option_discussions_2",
                "clan_member_manage_troops",
                "cheat_choice",
            });
        }

        // 发送信使
        public static void SendMessenger(Hero hero)
        {
            if (!SendedMessenger(hero))
            {
                Messengers.AddLast(new Messenger(hero));
            }
        }

        // 是否已经发送信使
        public static bool SendedMessenger(Hero hero)
        {
            return Messengers.SingleOrDefault((Messenger x) => x.Hero == hero) != null;
        }


        // 每小时检查
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
                this.StartMeeting(messenger.Hero);
            }
        }


        private class Messenger
        {
            public Hero Hero { get; protected set; }

            public bool Ready { get; protected set; }

            private const float SpeedPerHour = 30f;
            private int Hours = 0;

            public Messenger(Hero hero)
            {
                this.Hero = hero;
                this.Ready = false;
            }

            public void HourlyTick()
            {
                this.Hours += 1;

                if (this.Ready) return;

                Vec2 diff = Hero.MainHero.GetHeroPosition() - this.Hero.GetHeroPosition();
                if (diff.Length <= SpeedPerHour * (float)this.Hours)
                {
                    this.Ready = true;
                    MCLog.Info("信使会面: " + this.Hero.ToString()+" -> " + Hours.ToString() + "小时");
                }
            }
        }


        private static LinkedList<Messenger> Messengers = new LinkedList<Messenger>();





        // 是否在会话中
        public static bool MeetingInProgress
        {
            get
            {
                return meetingEncounter != null;
            }
        }

        

        private void OnConversationEnded(IEnumerable<CharacterObject> character)
        {
            if (meetingEncounter != null)
            {
                PlayerEncounter.Finish(false);
                meetingEncounter = null;
                meetingHero = null;
                AccessTools.Property(typeof(Campaign), "PlayerEncounter").SetValue(Campaign.Current, keepEncounter);
                keepEncounter = null;
                AccessTools.Property(typeof(Campaign), "LocationEncounter").SetValue(Campaign.Current, keepLocation);
                keepLocation = null;
                Hero.MainHero.PartyBelongedTo.CurrentSettlement = keepSettlement;
                keepSettlement = null;
            }
        }

        private void StartMeeting(Hero hero)
        {
            Hero player = Hero.MainHero;
            MobileParty partyBelongedTo = player.PartyBelongedTo;
            MobileParty partyBelongedTo2 = hero.PartyBelongedTo;


            PartyBase playerParty = (partyBelongedTo != null) ? partyBelongedTo.Party : null;
            PartyBase heroParty = (partyBelongedTo2 != null) ? partyBelongedTo2.Party : null;

            // 没有团队 或者 属于主角团队
            if (heroParty == null || heroParty == playerParty)
            {
                // 英雄的家定居点
                Settlement homeSettlement = hero.HomeSettlement;
                PartyBase partyBase;

                // 如存在英雄定居点，则取定居点所属的团队
                if ((partyBase = ((homeSettlement != null) ? homeSettlement.Party : null)) == null)
                {
                    // 如不存在英雄定居点，则取已知的最近定居点所属团队
                    Settlement lastKnownClosestSettlement = hero.LastKnownClosestSettlement;
                    if ((partyBase = ((lastKnownClosestSettlement != null) ? lastKnownClosestSettlement.Party : null)) == null)
                    {
                        // 如果最近定居点不存在， 则取玩家定居点所属团队
                        Settlement homeSettlement2 = player.HomeSettlement;
                        partyBase = ((homeSettlement2 != null) ? homeSettlement2.Party : null);
                    }
                }
                // 英雄所属团队
                heroParty = partyBase;
            }

            // 不是流浪者 或者 英雄存在所属团队
            if (!hero.IsWanderer || heroParty != null)
            {
                keepEncounter = PlayerEncounter.Current;
                keepLocation = (LocationEncounter)AccessTools.Property(typeof(Campaign), "LocationEncounter").GetValue(Campaign.Current);
                keepSettlement = player.PartyBelongedTo.CurrentSettlement;

                // 流浪者 团队 并且流浪者 当前定居点不为空 ,则切换玩家当前定居点
                if (heroParty == null && hero.CurrentSettlement != null)
                {
                    player.PartyBelongedTo.CurrentSettlement = hero.CurrentSettlement;
                }
                
                // 开始遭遇
                PlayerEncounter.Start();

                PlayerEncounter.Current.SetupFields(playerParty, heroParty ?? playerParty);
                meetingEncounter = PlayerEncounter.Current;
                meetingHero = hero;
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.CurrentConversationContext = ConversationContext.Default;
                AccessTools.Field(typeof(PlayerEncounter), "_mapEventState").SetValue(PlayerEncounter.Current, PlayerEncounterState.Begin);
                AccessTools.Field(typeof(PlayerEncounter), "_stateHandled").SetValue(PlayerEncounter.Current, true);
                AccessTools.Field(typeof(PlayerEncounter), "_meetingDone").SetValue(PlayerEncounter.Current, true);
            }

            CampaignMission.OpenConversationMission(
                new ConversationCharacterData(CharacterObject.PlayerCharacter, playerParty),
                new ConversationCharacterData(hero.CharacterObject, heroParty),
                "", ""
            );
        }

        private static PlayerEncounter meetingEncounter = null;   // 会话

        private static Hero meetingHero = null;   // 会面的英雄

        private static PlayerEncounter keepEncounter = null;  // 会话

        private static LocationEncounter keepLocation = null;  // 位置

        private static Settlement keepSettlement = null;  // 定居点
    }



    public static class MyTalkExtensions
    {
        // 哪些角色可以聊天
        public static bool CanTalkTo(this Hero hero)
        {
            Hero mainHero = Hero.MainHero;
            return hero != mainHero && hero.IsAlive && hero.IsActive 
                && !hero.IsPrisoner && !mainHero.IsPrisoner
                && (hero.PartyBelongedTo != null ? hero.PartyBelongedTo.MapEvent : null) == null
                && (mainHero.PartyBelongedTo != null ? mainHero.PartyBelongedTo.MapEvent : null) == null;
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
            IMapPoint mapPoint = hero.GetMapPoint();
            if (mapPoint != null) return mapPoint.Position2D;
            if (hero.HomeSettlement != null) return hero.HomeSettlement.Position2D;
            return Vec2.Zero;
        }
    }


    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
    public class EncyclopediaHeroPageVMPatch
    {
        public static bool Prefix(EncyclopediaHeroPageVM __instance)
        {
            Hero hero = __instance.Obj as Hero;
            if (hero.CanTalkTo())
            {
                __instance.HeroCharacter = new HeroViewModelEx(hero, CharacterViewModel.StanceTypes.EmphasizeFace);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FlattenedTroopRoster), "GenerateUniqueNoFromParty")]
    public class FlattenedTroopRosterPatch
    {
        public static bool Prefix(MobileParty party, int troopIndex, ref int __result)
        {
            int? num;
            if (party == null)
            {
                num = null;
            }
            else
            {
                PartyBase party2 = party.Party;
                num = ((party2 != null) ? new int?(party2.Index) : null);
            }
            int? num2 = num;
            __result = (num2.GetValueOrDefault(1) * 999983 + troopIndex * 100003) % 616841;
            return false;
        }
    }

    [HarmonyPatch(typeof(GameMenuManager), "ExitToLast")]
    public class GameMenuManagerPatch
    {
        public static bool Prefix()
        {
            if (Campaign.Current.CurrentMenuContext != null)
            {
                MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
                if (mapState != null)
                {
                    mapState.ExitMenuMode();
                }
            }
            return false;
        }
    }





    [PrefabExtension("EncyclopediaHeroPage", "descendant::TextWidget[@Text='@SkillsText']")]
    internal sealed class EncyclopediaHeroPagePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Prepend;

        private readonly XmlDocument document;

        public EncyclopediaHeroPagePrefabExtension()
        {
            document = new XmlDocument();
            document.LoadXml(@"
                <Widget WidthSizePolicy=""CoverChildren"" HeightSizePolicy=""CoverChildren"" VerticalAlignment=""Top"" HorizontalAlignment=""Center"" >
                    <Children>
                        <ButtonWidget DataSource=""{HeroCharacter}"" DoNotPassEventsToChildren=""true"" WidthSizePolicy=""Fixed"" HeightSizePolicy=""Fixed"" SuggestedWidth=""227"" SuggestedHeight=""40"" Brush=""ButtonBrush2"" UpdateChildrenStates=""true"" Command.Click=""CallToTalk"" IsVisible=""@CanTalkTo"" IsEnabled=""@WillNotTalk"">
                            <Children>
                                <TextWidget WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent"" Brush=""Kingdom.GeneralButtons.Text"" Text=""@CallToTalkText"" />
                            </Children>
                        </ButtonWidget>
                    </Children>
                </Widget>
            ");
        }
        
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => document;
    }

    // [ViewModelMixin(nameof(EncyclopediaHeroPageVM.RefreshValues))]
    [ViewModelMixin("Refresh")] // or [ViewModelMixin(nameof(MapInfoVM.Refresh))] // if the method is public
    internal sealed class EncyclopediaHeroPageVMMixin : BaseViewModelMixin<EncyclopediaHeroPageVM>
    {
        // private bool _isMessengerAvailable;
        // private HintViewModel? _sendMessengerHint;
        // private readonly Hero _hero;
        // private readonly GoldCost _sendMessengerCost;

        // private static readonly TextObject _TSendMessengerText = new("{=cXfcwzPp}Send Messenger");

        private readonly Hero hero;

        private readonly string btnText;

        public EncyclopediaHeroPageVMMixin(EncyclopediaHeroPageVM vm) : base(vm)
        {
            // this.hero = hero;
            hero = vm.Obj as Hero;
            this.btnText = new TextObject("Talk to me!", null).ToString();

            //_hero = (vm.Obj as Hero)!;
            //_sendMessengerCost = DiplomacyCostCalculator.DetermineCostForSendingMessenger();
            //SendMessengerCost = (int)_sendMessengerCost.Value;
            //SendMessengerActionName = _TSendMessengerText.ToString();
            vm.RefreshValues();
        }

        public override void OnRefresh()
        {
            //ViewModel?.RefreshValues();
            // this is called before the constructor the first time
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hero is null)
            {
                return;
            }
            CallToTalk();
        }

        [DataSourceMethod]
        public void CallToTalk()
        {
            MyTalkBehavior.SendMessenger(this.hero);
            MBTextManager.SetTextVariable("HeroName", this.hero.Name);
            MCLog.Info("{HeroName} 即将回应您的召唤……");
            base.OnPropertyChanged("WillNotTalk");
            OnRefresh();
        }

        [DataSourceProperty]
        public string CallToTalkText
        {
            get
            {
                return this.btnText;
            }
        }

        [DataSourceProperty]
        public bool CanTalkTo
        {
            get
            {
                return this.hero.CanTalkTo();
            }
        }

        [DataSourceProperty]
        public bool WillNotTalk
        {
            get
            {
                return !MyTalkBehavior.SendedMessenger(this.hero);
            }
        }
    }

    public class HeroViewModelEx : HeroViewModel
    {
        private readonly Hero hero;

        private readonly string btnText;

        public HeroViewModelEx(Hero hero, StanceTypes stance = 0) : base(stance)
        {
            this.hero = hero;
            this.btnText = new TextObject("Talk to me!", null).ToString();
        }

        public void CallToTalk()
        {
            MyTalkBehavior.SendMessenger(this.hero);
            MBTextManager.SetTextVariable("HeroName", this.hero.Name);
            MCLog.Info("{HeroName} 即将回应您的召唤……");
            base.OnPropertyChanged("WillNotTalk");
        }

        [DataSourceProperty]
        public string CallToTalkText
        {
            get
            {
                return this.btnText;
            }
        }

        [DataSourceProperty]
        public bool CanTalkTo
        {
            get
            {
                return this.hero.CanTalkTo();
            }
        }

        [DataSourceProperty]
        public bool WillNotTalk
        {
            get
            {
                return !MyTalkBehavior.SendedMessenger(this.hero);
            }
        }


    }
}
