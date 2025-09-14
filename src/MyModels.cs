using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.Settlements;

namespace MB2MultiCheats
{
    internal class MyCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
    {
        // 六维最大属性点 10 -> 30
        public override int MaxAttribute
        {
            get
            {
                return (int)GlobalSettings<MySettings>.Instance.MaxAttribute;
            }
        }

        // 技能最大专精点  5 -> 15
        public override int MaxFocusPerSkill
        {
            get
            {
                return (int)GlobalSettings<MySettings>.Instance.MaxFocusPerSkill;
            }
        }

        // 玩家家族学习效率提升倍率
        public override float CalculateLearningRate(Hero hero, SkillObject skill)
        {
            if (hero.Clan == Clan.PlayerClan)
                return base.CalculateLearningRate(hero, skill) * (float)GlobalSettings<MySettings>.Instance.ExtraLearningRate;
            else
                return base.CalculateLearningRate(hero, skill);
        }
    }

    internal class MySmithingModel : DefaultSmithingModel
    {
        // 精炼体力消耗
        public override int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero)
        {
            return (bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForRefining(ref refineFormula, hero);
        }

        // 熔炼体力消耗
        public override int GetEnergyCostForSmelting(ItemObject item, Hero hero)
        {
            return (bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForSmelting(item, hero);
        }

        // 锻造体力消耗
        public override int GetEnergyCostForSmithing(ItemObject item, Hero hero)
        {
            return (bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost ? 0 : base.GetEnergyCostForSmithing(item, hero);
        }

        // 配件解锁加成
        public override float ResearchPointsNeedForNewPart(int totalPartCount, int openedPartCount)
        {
            return base.ResearchPointsNeedForNewPart(totalPartCount, openedPartCount) / (float)GlobalSettings<MySettings>.Instance.NewPartUnlockRate;
        }

        // 锻造经验加成
        public override int GetSkillXpForSmithingInFreeBuildMode(ItemObject item)
        {
            return base.GetSkillXpForSmithingInFreeBuildMode(item) * (int)GlobalSettings<MySettings>.Instance.FreeSmithingXpRate;
        }
    }

    internal class MyPregnancyModel : DefaultPregnancyModel
    {
        // 生育难产死亡
        public override float MaternalMortalityProbabilityInLabor
        {
            get
            {
                return (bool)GlobalSettings<MySettings>.Instance.CloseMaternalMortality ? 0f : 0.015f;
            }
        }

        // 生育小产死亡
        public override float StillbirthProbability
        {
            get
            {
                return (bool)GlobalSettings<MySettings>.Instance.CloseStillbirth ? 0f : 0.01f;
            }
        }
    }

    internal class MyBuildingConstructionModel : DefaultBuildingConstructionModel
    {
        // 定居点建设速度增益
        public override int GetBoostAmount(Town town)
        {
            if (town?.OwnerClan?.Leader != null && town.OwnerClan.Leader.IsHumanPlayerCharacter)
            {
                return base.GetBoostAmount(town) + (town.IsCastle ? CastleBoostBonus : TownBoostBonus) * (GlobalSettings<MySettings>.Instance.DailySettlementBoostBonus - 1);
            }
            return base.GetBoostAmount(town);
        }
    }

    internal class MyBattleRewardModel : DefaultBattleRewardModel
    {
        // 战利品存在优质前缀, 则均分优质前缀概率
        public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
        {
            EquipmentElement randomItem = base.GetLootedItemFromTroop(character, targetValue);
            if (randomItem.ItemModifier != null && randomItem.ItemModifier.PriceMultiplier > 1f && MCRand.RandBool(GlobalSettings<MySettings>.Instance.GainLootedItemRate))
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
            if (MCRand.RandBool(GlobalSettings<MySettings>.Instance.GainLootedItemRate))
                return base.GetExpectedLootedItemValue(character) * (float)character.Level;
            else
                return base.GetExpectedLootedItemValue(character);
        }
    }



}
