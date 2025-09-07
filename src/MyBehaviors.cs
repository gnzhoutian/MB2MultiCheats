using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using MCM.Abstractions.Base.Global;
using NetworkMessages.FromServer;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace MB2MultiCheats
{
    public class MyBehaviors : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, TournamentExtraPrize);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, DailyAddLoyaltyForSettlement);
            CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, LevelupRewordAttributeAndFocus);
            CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, GainedSkillAutoAddPerks);
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, CompanionAppendAttributeAndFocus);
        }

        // 竞技大赛奖励
        private void TournamentExtraPrize(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
        {
            if (winner != null && winner.IsHero && winner.IsPlayerCharacter)
            {
                CharacterObject character_obj = new CharacterObject();
                ItemObject item_obj = new ItemObject();
                TextObject text_obj = new TextObject();
                Random rand = new Random();

                int min = (int)GlobalSettings<MySettings>.Instance.ExtraRewardTroopMin;
                int range = (int)GlobalSettings<MySettings>.Instance.ExtraRewardTroopRange;
                int rate = (int)GlobalSettings<MySettings>.Instance.ExtraRewardItemRate;
                int num = Math.Min(rand.Next(min, min + range + 1), MobileParty.MainParty.LimitedPartySize - MobileParty.MainParty.Party.NumberOfAllMembers);

                List<string> reward_items = new List<string>() { "mc_item_dragon_bracer", "mc_item_shadow_horse", "mc_item_dragon_scale_horse_barding", "mc_item_sunset_bow" , "mc_item_eagle_feather_arrows"};

                if (num > 0)
                {
                    character_obj = MBObjectManager.Instance.GetObject<CharacterObject>("mc_troop_dragon_guard");
                    MBTextManager.SetTextVariable("MC_Main_Reward_Troop_Name", character_obj.ToString());
                    MBTextManager.SetTextVariable("MC_Main_Reward_Troop_Num", num.ToString());
                    text_obj = new TextObject("{=mcMainBehaviorRewardTroop}{MC_Main_Reward_Troop_Num} {MC_Main_Reward_Troop_Name} admire your heroic spirit and decide to follow you!");

                    MobileParty.MainParty.AddElementToMemberRoster(character_obj, num);
                    InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Green).Value));
                }

                foreach (string reward_item in reward_items)
                {
                    if (rand.Next(100) < rate)
                    {
                        item_obj = MBObjectManager.Instance.GetObject<ItemObject>(reward_item);
                        MBTextManager.SetTextVariable("MC_Main_Reward_Item_Name", item_obj.Name.ToString());
                        text_obj = new TextObject("{=mcMainBehaviorRewardItem}You performed excellent in the competition and received additional rewards: {MC_Main_Reward_Item_Name}");

                        MobileParty.MainParty.ItemRoster.AddToCounts(item_obj, 1);
                        InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Green).Value));
                    }
                }
            }
        }

        // 定居点忠诚奖励
        private void DailyAddLoyaltyForSettlement(Settlement settlement)
        {
            int loyalty = (int)GlobalSettings<MySettings>.Instance.DailySettlementLoyalty;
            if (loyalty > 0)
            {
                if (settlement.IsTown || settlement.IsCastle)
                {
                    if (settlement.OwnerClan != null && settlement.OwnerClan.Leader != null && settlement.OwnerClan.Leader.IsHumanPlayerCharacter)
                    {
                        settlement.Town.Loyalty += loyalty;
                        MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Name", settlement.ToString());
                        MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Now", settlement.Town.Loyalty.ToString("0.0"));
                        MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Add", loyalty.ToString());
                        TextObject text_obj = new TextObject(
                            "{=mcMainBehaviorRewardLoyalty}Clan settlement loyalty：{MC_Main_Reward_Loyalty_Name} {MC_Main_Reward_Loyalty_Now}(+{MC_Main_Reward_Loyalty_Add})");

                        InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Gray).Value));
                    }
                }
            }
        }

        // 角色升级属性点专精点奖励
        private void LevelupRewordAttributeAndFocus(Hero hero, bool crap)
        {
            if (hero.Clan == Clan.PlayerClan)
            {
                hero.HeroDeveloper.UnspentAttributePoints += (int)GlobalSettings<MySettings>.Instance.LevelupExtraAttribute;
                hero.HeroDeveloper.UnspentFocusPoints += (int)GlobalSettings<MySettings>.Instance.LevelupExtraFocus;
            }
        }

        // 技能升级自动激活Perk点
        private void GainedSkillAutoAddPerks(Hero hero, SkillObject skill, int crap1, bool crap2)
        {
            if (hero.Clan == Clan.PlayerClan && (bool)GlobalSettings<MySettings>.Instance.AutoTakeBothPerks)
            {
                TextObject text_obj = new TextObject();
                MBTextManager.SetTextVariable("MC_Main_Active_Perk_Hero", hero.ToString());

                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk.Skill == skill && !hero.GetPerkValue(perk) && hero.GetSkillValue(skill) >= perk.RequiredSkillValue)
                    {
                        if (perk != null)
                        {
                            MBTextManager.SetTextVariable("MC_Main_Active_Perk_Name", perk.ToString());
                            text_obj = new TextObject("{=mcMainBehaviorActivePerk}{MC_Main_Active_Perk_Hero} active perk {MC_Main_Active_Perk_Name}");

                            hero.HeroDeveloper.AddPerk(perk);
                            InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Green).Value));
                        }
                        if (perk.AlternativePerk != null)
                        {
                            MBTextManager.SetTextVariable("MC_Main_Active_Perk_Name", perk.AlternativePerk.ToString());
                            text_obj = new TextObject("{=mcMainBehaviorActivePerk}{MC_Main_Active_Perk_Hero} active perk {MC_Main_Active_Perk_Name}");

                            hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                            InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Green).Value));
                        }
                    }
                }
            }
        }


        // 家族成员对话后追加属性点专精点等级奖励
        private void CompanionAppendAttributeAndFocus(IEnumerable<CharacterObject> heros)
        {
            if ((bool)GlobalSettings<MySettings>.Instance.CompanionAppendAttributeAndFocus) {
                foreach (CharacterObject obj in heros)
                {
                    if (obj.IsHero && !obj.IsPlayerCharacter)
                    {
                        Hero hero = obj.HeroObject;
                        if (hero.Clan == Clan.PlayerClan && hero.Age > 18.0f && hero.HeroDeveloper.UnspentAttributePoints == 0 && hero.HeroDeveloper.UnspentFocusPoints == 0)
                        {
                            int attrPoints = hero.Level * (int)GlobalSettings<MySettings>.Instance.LevelupExtraAttribute;
                            int focusPoints = hero.Level * (int)GlobalSettings<MySettings>.Instance.LevelupExtraFocus;

                            hero.HeroDeveloper.UnspentAttributePoints += attrPoints;
                            hero.HeroDeveloper.UnspentFocusPoints += focusPoints;

                            TextObject text_obj = new TextObject();
                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Hero", hero.ToString());
                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Attr", attrPoints.ToString());
                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Focus", focusPoints.ToString());

                            text_obj = new TextObject(
                                "{=mcMainBehaviorRewardPoints}{MC_Main_Reward_Points_Hero} append level rewards：{MC_Main_Reward_Points_Attr} attribute points，{MC_Main_Reward_Points_Focus} focus points");
                            InformationManager.DisplayMessage(new InformationMessage(text_obj.ToString(), new Color?(Colors.Green).Value));
                        }
                    }
                }
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
