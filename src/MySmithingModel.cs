using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using MCM.Abstractions.Base.Global;

namespace MultiCheats
{
    class MySmithingModel : DefaultSmithingModel
    {
        // 精炼体力消耗
        public override int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero)
        {
            if ((bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost)
            {
                return 0;
            }
            return base.GetEnergyCostForRefining(ref refineFormula, hero);
        }

        // 熔炼体力消耗
        public override int GetEnergyCostForSmelting(ItemObject item, Hero hero)
        {
            if ((bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost)
            {
                return 0;
            }
            return base.GetEnergyCostForSmelting(item, hero);
        }

        // 锻造体力消耗
        public override int GetEnergyCostForSmithing(ItemObject item, Hero hero)
        {
            if ((bool)GlobalSettings<MySettings>.Instance.SmithingWithoutEnergyCost)
            {
                return 0;
            }
            return base.GetEnergyCostForSmithing(item, hero);
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
}
