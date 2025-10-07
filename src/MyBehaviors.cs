using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace MB2MultiCheats
{
    internal class MyBehaviors : CampaignBehaviorBase
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
                int rate = MySettings.Instance.ExtraRewardItemRate;
                int range = MySettings.Instance.ExtraRewardItemRange;

                List<string> reward_items = new List<string>() { "mc_item_dragon_bracer", "mc_item_eagle_feather_arrows"};

                foreach (string reward_item in reward_items)
                {
                    if (MCRand.RandBool(rate))
                    {
                        int item_num = MCRand.RandNum(1, range);
                        ItemObject item_obj = MBObjectManager.Instance.GetObject<ItemObject>(reward_item);
                        MobileParty.MainParty.ItemRoster.AddToCounts(item_obj, item_num);

                        MBTextManager.SetTextVariable("MC_Main_Reward_Item_Name", item_obj.Name.ToString());
                        MBTextManager.SetTextVariable("MC_Main_Reward_Item_Num", item_num.ToString());
                        MCLog.Info("{=mcMainBehaviorRewardItem}You performed excellent in the competition and received additional rewards: {MC_Main_Reward_Item_Name}x{MC_Main_Reward_Item_Num}");
                    }
                }
            }
        }

        // 定居点忠诚奖励
        private void DailyAddLoyaltyForSettlement(Settlement settlement)
        {
            int loyalty = (int)MySettings.Instance.DailySettlementLoyalty;
            if (loyalty > 0)
            {
                if ((settlement.IsTown || settlement.IsCastle) && settlement?.OwnerClan?.Leader != null && settlement.OwnerClan.Leader.IsHumanPlayerCharacter)
                {
                    settlement.Town.Loyalty += loyalty;

                    MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Name", settlement.ToString());
                    MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Now", settlement.Town.Loyalty.ToString("0.0"));
                    MBTextManager.SetTextVariable("MC_Main_Reward_Loyalty_Add", loyalty.ToString());
                    MCLog.Debug("{=mcMainBehaviorRewardLoyalty}Clan settlement loyalty：{MC_Main_Reward_Loyalty_Name} {MC_Main_Reward_Loyalty_Now}(+{MC_Main_Reward_Loyalty_Add})");
                }
            }
        }

        // 角色升级属性点专精点奖励
        private void LevelupRewordAttributeAndFocus(Hero hero, bool crap)
        {
            if (hero.Clan == Clan.PlayerClan)
            {
                hero.HeroDeveloper.UnspentAttributePoints += (int)MySettings.Instance.LevelupExtraAttribute;
                hero.HeroDeveloper.UnspentFocusPoints += (int)MySettings.Instance.LevelupExtraFocus;
            }
        }

        // 技能升级自动激活Perk点
        private void GainedSkillAutoAddPerks(Hero hero, SkillObject skill, int crap1, bool crap2)
        {
            if (hero.Clan == Clan.PlayerClan && (bool)MySettings.Instance.AutoTakeBothPerks)
            {
                MBTextManager.SetTextVariable("MC_Main_Active_Perk_Hero", hero.ToString());

                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk.Skill == skill && !hero.GetPerkValue(perk) && hero.GetSkillValue(skill) >= perk.RequiredSkillValue)
                    {
                        if (perk != null)
                        {
                            hero.HeroDeveloper.AddPerk(perk);

                            MBTextManager.SetTextVariable("MC_Main_Active_Perk_Name", perk.ToString());
                            MCLog.Info("{=mcMainBehaviorActivePerk}{MC_Main_Active_Perk_Hero} active perk {MC_Main_Active_Perk_Name}");
                        }
                        if (perk.AlternativePerk != null)
                        {
                            hero.HeroDeveloper.AddPerk(perk.AlternativePerk);

                            MBTextManager.SetTextVariable("MC_Main_Active_Perk_Name", perk.AlternativePerk.ToString());
                            MCLog.Info("{=mcMainBehaviorActivePerk}{MC_Main_Active_Perk_Hero} active perk {MC_Main_Active_Perk_Name}");
                        }
                    }
                }
            }
        }

        // 家族成员对话后追加属性点专精点等级奖励
        private void CompanionAppendAttributeAndFocus(IEnumerable<CharacterObject> heros)
        {
            if ((bool)MySettings.Instance.CompanionAppendAttributeAndFocus) {
                foreach (CharacterObject obj in heros)
                {
                    if (!obj.IsPlayerCharacter && obj.IsHero && obj.HeroObject.Clan == Clan.PlayerClan && obj.HeroObject.IsActive)
                    {
                        Hero hero = obj.HeroObject;
                        int attrPoints = 0;
                        int focusPoints = 0;

                        if (hero.HeroDeveloper.UnspentAttributePoints == 0)
                            attrPoints = hero.Level * (int)MySettings.Instance.LevelupExtraAttribute;

                        if (hero.HeroDeveloper.UnspentFocusPoints == 0)
                            focusPoints = hero.Level * (int)MySettings.Instance.LevelupExtraFocus;

                        if (attrPoints != 0 || focusPoints != 0)
                        {
                            hero.HeroDeveloper.UnspentAttributePoints += attrPoints;
                            hero.HeroDeveloper.UnspentFocusPoints += focusPoints;

                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Hero", hero.ToString());
                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Attr", attrPoints.ToString());
                            MBTextManager.SetTextVariable("MC_Main_Reward_Points_Focus", focusPoints.ToString());
                            MCLog.Info("{=mcMainBehaviorRewardPoints}{MC_Main_Reward_Points_Hero} append level rewards：{MC_Main_Reward_Points_Attr} attribute points，{MC_Main_Reward_Points_Focus} focus points");
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
