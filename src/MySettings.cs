using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace MultiCheats
{
    internal sealed class MySettings: AttributeGlobalSettings<MySettings>
    {
        public override string Id
        {
            get
            {
                return "MultiCheats";
            }
        }
        public override string DisplayName
        {
            get
            {
                return "MultiCheats";
            }
        }

        public override string FolderName
        {
            get
            {
                return "MultiCheats";
            }
        }

        public override string FormatType
        {
            get
            {
                return "json";
            }
        }
        
        [SettingPropertyInteger("兵种奖励基数", 0, 9, Order = 0, RequireRestart = false, HintText = "亲卫兵种奖励最少数量，默认为0名，推荐1名")]
        [SettingPropertyGroup("竞技大赛", GroupOrder = 0)]
        public int ExtraRewardTroopMin { get; set; } = 0;

        [SettingPropertyInteger("兵种奖励区间", 0, 9, Order = 1, RequireRestart = false, HintText = "亲卫兵种奖励随机区间，默认为0名，推荐2名")]
        [SettingPropertyGroup("竞技大赛", GroupOrder = 0)]
        public int ExtraRewardTroopRange { get; set; } = 0;

        [SettingPropertyInteger("物品奖励概率", 0, 100, Order = 2, RequireRestart = false, HintText = "神级物品奖励概率，默认为0%，推荐20%")]
        [SettingPropertyGroup("竞技大赛", GroupOrder = 0)]
        public int ExtraRewardItemRate { get; set; } = 0;
        
        
        [SettingPropertyInteger("六维最大属性", 10, 20, Order = 0, RequireRestart = false, HintText = "六维最大属性点，默认为10点，推荐20点")]
        [SettingPropertyGroup("技能升级", GroupOrder = 1)]
        public int MaxAttribute { get; set; } = 10;

        [SettingPropertyInteger("技能最大专精", 5, 10, Order = 1, RequireRestart = false, HintText = "技能最大专精点，默认为5点，推荐10点")]
        [SettingPropertyGroup("技能升级", GroupOrder = 1)]
        public int MaxFocusPerSkill { get; set; } = 5;
        
        [SettingPropertyInteger("学习效率加成", 1, 100, Order = 2, RequireRestart = false, HintText = "家族成员技能学习效率后台加成倍率，默认为1倍，推荐10倍")]
        [SettingPropertyGroup("技能升级", GroupOrder = 1)]
        public int ExtraLearningRate { get; set; } = 1;

        [SettingProperty("自动激活特性", Order = 3, RequireRestart = false, HintText = "家族成员技能升级时自动激活Perk，默认为关，推荐开")]
        [SettingPropertyGroup("技能升级", GroupOrder = 1)]
        public bool AutoTakeBothPerks { get; set; } = false;


        [SettingPropertyInteger("每级额外属性", 0, 9, Order = 0, RequireRestart = false, HintText = "每级后台额外奖励属性点，默认为0点，推荐2点")]
        [SettingPropertyGroup("角色升级", GroupOrder = 2)]
        public int LevelupExtraAttribute { get; set; } = 0;

        [SettingPropertyInteger("每级额外专精", 0, 9, Order = 1, RequireRestart = false, HintText = "每级后台额外奖励专精点，默认为0点，推荐2点")]
        [SettingPropertyGroup("角色升级", GroupOrder = 2)]
        public int LevelupExtraFocus { get; set; } = 0;

        [SettingProperty("同伴追加点数", Order = 3, RequireRestart = false, HintText = "家族成员属性专精点同时为0时，对话后追加等级额外点数，默认为关，推荐开")]
        [SettingPropertyGroup("角色升级", GroupOrder = 2)]
        public bool CompanionAppendAttributeAndFocus { get; set; } = false;

        [SettingPropertyInteger("锻造经验加成", 1, 100, Order = 0, RequireRestart = false, HintText = "自由锻造经验加成倍率，默认为1倍，推荐10倍")]
        [SettingPropertyGroup("锻造", GroupOrder = 3)]
        public int FreeSmithingXpRate { get; set; } = 1;
        
        [SettingPropertyInteger("配件解锁加成", 1, 100, Order = 1, RequireRestart = false, HintText = "配件解锁速度加成倍率，默认为1倍，推荐10倍")]
        [SettingPropertyGroup("锻造", GroupOrder = 3)]
        public int NewPartUnlockRate { get; set; } = 1;


        [SettingPropertyInteger("每日奖励忠诚", 0, 9, Order = 0, RequireRestart = false, HintText = "家族定居点每日后台奖励忠诚，默认为0点，推荐2点")]
        [SettingPropertyGroup("定居点", GroupOrder = 4)]
        public int DailySettlementLoyalty { get; set; } = 0;
    }
}

