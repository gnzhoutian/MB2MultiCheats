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
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;


namespace MB2MultiCheats
{

    class MyCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
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
            {
                return base.CalculateLearningRate(hero, skill) * (float)GlobalSettings<MySettings>.Instance.ExtraLearningRate;
            }
            else
            {
                return base.CalculateLearningRate(hero, skill);
            }
        }
    }
}
