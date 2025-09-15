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

namespace MultiCheats
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
                Random rand = new Random();
                int min = (int)GlobalSettings<MySettings>.Instance.ExtraRewardTroopMin;
                int range = (int)GlobalSettings<MySettings>.Instance.ExtraRewardTroopRange;
                int rate = (int)GlobalSettings<MySettings>.Instance.ExtraRewardItemRate;
                int num = Math.Min(rand.Next(min, min + range + 1), MobileParty.MainParty.LimitedPartySize - MobileParty.MainParty.Party.NumberOfAllMembers);

                if (num > 0)
                {
                    MobileParty.MainParty.AddElementToMemberRoster(MBObjectManager.Instance.GetObject<CharacterObject>("mc_elite_dragon"), num);
                    InformationManager.DisplayMessage(new InformationMessage(string.Format("{0}名龙啸卫仰慕您的英姿，决定追随您！", num), new Color?(Colors.Green).Value));
                }

                if (rand.Next(100) < rate)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mc_item_armor_longyin"), 1);
                    InformationManager.DisplayMessage(new InformationMessage("您在竞技大赛中表现出色，额外奖励：龙吟护腕", new Color?(Colors.Green).Value));
                }

                if (rand.Next(100) < rate)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mc_item_weapon_luori"), 1);
                    InformationManager.DisplayMessage(new InformationMessage("您在竞技大赛中表现出色，额外奖励：落日弓", new Color?(Colors.Green).Value));
                }

                if (rand.Next(100) < rate)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mc_item_weapon_diaoling"), 1);
                    InformationManager.DisplayMessage(new InformationMessage("您在竞技大赛中表现出色，额外奖励：雕翎箭", new Color?(Colors.Green).Value));
                }

                if (rand.Next(100) < rate)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mc_item_horse_jueying"), 1);
                    InformationManager.DisplayMessage(new InformationMessage("您在竞技大赛中表现出色，额外奖励：绝影", new Color?(Colors.Green).Value));
                }

                if (rand.Next(100) < rate)
                {
                    MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mc_item_armor_jueying"), 1);
                    InformationManager.DisplayMessage(new InformationMessage("您在竞技大赛中表现出色，额外奖励：龙鳞战甲", new Color?(Colors.Green).Value));
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
                        InformationManager.DisplayMessage(new InformationMessage("家族定居点忠诚：" + settlement.ToString() + " " + settlement.Town.Loyalty.ToString("0.0") + "(+" + loyalty.ToString() + ")", new Color?(Colors.Gray).Value));
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
                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk.Skill == skill && !hero.GetPerkValue(perk) && hero.GetSkillValue(skill) >= perk.RequiredSkillValue)
                    {
                        if (perk != null)
                        {
                            hero.HeroDeveloper.AddPerk(perk);
                            InformationManager.DisplayMessage(new InformationMessage(hero.ToString() + " 激活特性 " + perk.ToString(), new Color?(Colors.Green).Value));
                        }
                        if (perk.AlternativePerk != null)
                        {
                            hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                            InformationManager.DisplayMessage(new InformationMessage(hero.ToString() + " 激活特性 " + perk.ToString(), new Color?(Colors.Green).Value));
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

                            InformationManager.DisplayMessage(new InformationMessage(hero.ToString() + " 追加等级奖励：属性" + attrPoints.ToString() + "点，专精"+ focusPoints.ToString() + '点', new Color?(Colors.Green).Value));
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
