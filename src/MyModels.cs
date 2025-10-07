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
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace MB2MultiCheats
{
    internal class MyCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
    {
        // 六维最大属性点 10 -> 30
        public override int MaxAttribute
        {
            get
            {
                return (int)MySettings.Instance.MaxAttribute;
            }
        }

        // 技能最大专精点  5 -> 15
        public override int MaxFocusPerSkill
        {
            get
            {
                return (int)MySettings.Instance.MaxFocusPerSkill;
            }
        }

        // 玩家家族学习效率提升倍率
        public override float CalculateLearningRate(Hero hero, SkillObject skill)
        {
            if (hero.Clan == Clan.PlayerClan)
            {
                return base.CalculateLearningRate(hero, skill) * (float)MySettings.Instance.ExtraLearningRate;
            }
            return base.CalculateLearningRate(hero, skill);
        }
    }

    internal class MySmithingModel : DefaultSmithingModel
    {
        // 精炼体力消耗
        public override int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero)
        {
            return (bool)MySettings.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForRefining(ref refineFormula, hero);
        }

        // 熔炼体力消耗
        public override int GetEnergyCostForSmelting(ItemObject item, Hero hero)
        {
            return (bool)MySettings.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForSmelting(item, hero);
        }

        // 锻造体力消耗
        public override int GetEnergyCostForSmithing(ItemObject item, Hero hero)
        {
            return (bool)MySettings.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForSmithing(item, hero);
        }

        // 配件解锁加成
        public override float ResearchPointsNeedForNewPart(int totalPartCount, int openedPartCount)
        {
            return base.ResearchPointsNeedForNewPart(totalPartCount, openedPartCount) / (float)MySettings.Instance.NewPartUnlockRate;
        }

        // 锻造经验加成
        public override int GetSkillXpForSmithingInFreeBuildMode(ItemObject item)
        {
            return base.GetSkillXpForSmithingInFreeBuildMode(item) * (int)MySettings.Instance.FreeSmithingXpRate;
        }
    }

    internal class MyPregnancyModel : DefaultPregnancyModel
    {
        // 生育难产死亡
        public override float MaternalMortalityProbabilityInLabor
        {
            get
            {
                return (bool)MySettings.Instance.CloseMaternalMortality ? 0f : 0.015f;
            }
        }

        // 生育小产死亡
        public override float StillbirthProbability
        {
            get
            {
                return (bool)MySettings.Instance.CloseStillbirth ? 0f : 0.01f;
            }
        }
    }

    internal class MyBuildingConstructionModel : DefaultBuildingConstructionModel
    {
        // 定居点建设速度增益
        public override int GetBoostAmount(Town town)
        {
            if (town?.OwnerClan?.Leader != null && town.OwnerClan.Leader.IsHumanPlayerCharacter && MySettings.Instance.DailySettlementBoostBonus > 0)
            {
                return base.GetBoostAmount(town) + (town.IsCastle ? CastleBoostBonus : TownBoostBonus) * MySettings.Instance.DailySettlementBoostBonus;
            }
            return base.GetBoostAmount(town);
        }
    }

    internal class MySettlementTaxModel: DefaultSettlementTaxModel
    {
        // 定居点总督税收增益
        public override ExplainedNumber CalculateTownTax(Town town, bool includeDescriptions = false)
        {
            ExplainedNumber result = base.CalculateTownTax(town, includeDescriptions);
            if ((town.IsTown || town.IsCastle) && town.Governor?.Clan == Clan.PlayerClan && MySettings.Instance.GainSettlementTaxByGovernor > 0)
            {
                if (town.Governor.GetPerkValue(DefaultPerks.Steward.PriceOfLoyalty))
                {
                    result.AddFactor((float)(town.Governor.GetSkillValue(DefaultSkills.Steward) * MySettings.Instance.GainSettlementTaxByGovernor) / 100f);
                }
            }
            return result;
        }
    }

    internal class MyClanTierModel : DefaultClanTierModel
    {
        // 同伴人数限制增益
        public override int GetCompanionLimit(Clan clan)
        {
            if (clan == Clan.PlayerClan)
            {
                return base.GetCompanionLimit(clan) * MySettings.Instance.GainCompanionSizeLimit;
            }
            return base.GetCompanionLimit(clan);
        }
    }

    internal class MyPartySizeLimitModel : DefaultPartySizeLimitModel
    {
        // 部队俘虏限制增益
        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            if (party.IsMobile && party.MobileParty.IsMainParty && MySettings.Instance.GainPrisonerSizeLimit > 1)
            {
                ExplainedNumber rst = base.GetPartyPrisonerSizeLimit(party, includeDescriptions);
                rst.Add(rst.BaseNumber * (float)(MySettings.Instance.GainPrisonerSizeLimit - 1), new TextObject("{=mcGainPrisonerSizeLimit}Prisoner extra gain"));
                return rst;
            }
            return base.GetPartyPrisonerSizeLimit(party, includeDescriptions);
        }
    }

    internal class MyPrisonerRecruitmentCalculationModel: DefaultPrisonerRecruitmentCalculationModel
    {
        // 部队俘虏招募增益
        public override int GetConformityChangePerHour(PartyBase party, CharacterObject troopToBoost)
        {
            if (party.IsMobile && party.MobileParty.IsMainParty && MySettings.Instance.GainPrisonerRecruitmentRate > 1)
            {
                return base.GetConformityChangePerHour(party, troopToBoost) * MySettings.Instance.GainPrisonerRecruitmentRate;
            }
            return base.GetConformityChangePerHour(party, troopToBoost);
        }
    }

    internal class MyBattleRewardModel : DefaultBattleRewardModel
    {
        // 战利品存在优质前缀, 则均分优质前缀概率
        public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
        {
            EquipmentElement randomItem = base.GetLootedItemFromTroop(character, targetValue);
            if (randomItem.ItemModifier != null && randomItem.ItemModifier.PriceMultiplier > 1f && MCRand.RandBool(MySettings.Instance.GainLootedItemRate))
            {
                MBList<ItemModifier> _itemModifiers = new MBList<ItemModifier>();
                foreach (ItemModifier itemModifier in randomItem.Item.ItemComponent.ItemModifierGroup.ItemModifiers)
                {
                    if (itemModifier.PriceMultiplier > 1f)
                        _itemModifiers.Add(itemModifier);
                }
                if (_itemModifiers.Count > 0)
                    randomItem = new EquipmentElement(randomItem.Item, _itemModifiers.GetRandomElement(), null, false);
            }
            return randomItem;
        }

        // 战利品最大价值增益
        public override float GetExpectedLootedItemValue(CharacterObject character)
        {
            if (MCRand.RandBool(MySettings.Instance.GainLootedItemRate))
            {
                return base.GetExpectedLootedItemValue(character) * (float)character.Level;
            }
            return base.GetExpectedLootedItemValue(character);
        }
    }

    internal class MyDiplomacyModel: DefaultDiplomacyModel
    {
        // 战争岂是儿戏
        public override int GetInfluenceCostOfProposingPeace(Clan proposingClan)
        {
            Clan playerClan = Clan.PlayerClan;
            if (playerClan.Tier >= 6 && playerClan.Kingdom != null && (proposingClan.Kingdom == playerClan.Kingdom || playerClan.IsAtWarWith(proposingClan)))
            {
                return base.GetInfluenceCostOfProposingPeace(proposingClan) * MySettings.Instance.WarIsNoJoke;
            }
            return base.GetInfluenceCostOfProposingPeace(proposingClan);
        }
    }
}
