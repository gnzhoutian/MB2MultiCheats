using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;


namespace MB2MultiCheats
{
    internal sealed class MySettings : AttributeGlobalSettings<MySettings>
    {
        public override string Id { get; } = MySubModule.ModuleName;

        public override string DisplayName { get; } = MySubModule.ModuleName;

        public override string FolderName { get; } = MySubModule.ModuleName;

        public override string FormatType { get; } = "json";

        [SettingPropertyInteger("{=mcConfigTournamentText1}Reward base for troop", 0, 9, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigTournamentDesc1}The min number of guards rewarded, the default is 0, and 1 is recommended")]
        [SettingPropertyGroup("{=mcConfigTournament}Tournament", GroupOrder = 0)]
        public int ExtraRewardTroopMin { get; set; } = 0;

        [SettingPropertyInteger("{=mcConfigTournamentText2}Reward range for troop", 0, 9, Order = 1, RequireRestart = false,
            HintText = "{=mcConfigTournamentDesc2}The random interval of guards rewarded, the default is 0, and 2 is recommended")]
        [SettingPropertyGroup("{=mcConfigTournament}Tournament", GroupOrder = 0)]
        public int ExtraRewardTroopRange { get; set; } = 0;

        [SettingPropertyInteger("{=mcConfigTournamentText3}Reward probability for item", 0, 100, Order = 2, RequireRestart = false,
            HintText = "{=mcConfigTournamentDesc3}The probability of rewarding god-level items, the default is 0%, and 20% is recommended")]
        [SettingPropertyGroup("{=mcConfigTournament}Tournament", GroupOrder = 0)]
        public int ExtraRewardItemRate { get; set; } = 0;


        [SettingPropertyInteger("{=mcConfigSkillLevelupText1}Max attribute", 10, 20, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigSkillLevelupDesc1}Six dimensions max attribute, the default is 10, and 20 is recommended")]
        [SettingPropertyGroup("{=mcConfigSkillLevelup}Skill Levelup", GroupOrder = 1)]
        public int MaxAttribute { get; set; } = 10;

        [SettingPropertyInteger("{=mcConfigSkillLevelupText2}Max focus per skill", 5, 10, Order = 1, RequireRestart = false,
            HintText = "{=mcConfigSkillLevelupDesc2}Max focus per skill, the default is 5, and 10 is recommended")]
        [SettingPropertyGroup("{=mcConfigSkillLevelup}Skill Levelup", GroupOrder = 1)]
        public int MaxFocusPerSkill { get; set; } = 5;

        [SettingPropertyInteger("{=mcConfigSkillLevelupText3}Learning rate", 1, 100, Order = 2, RequireRestart = false,
            HintText = "{=mcConfigSkillLevelupDesc3}The learning Rate of clan members, the default is 1, and 10 is recommended")]
        [SettingPropertyGroup("{=mcConfigSkillLevelup}Skill Levelup", GroupOrder = 1)]
        public int ExtraLearningRate { get; set; } = 1;

        [SettingProperty("{=mcConfigSkillLevelupText4}Auto take both perks", Order = 3, RequireRestart = false,
            HintText = "{=mcConfigSkillLevelupDesc4}Clan members auto take both perks when skill levelup, the default is false, and true is recommended")]
        [SettingPropertyGroup("{=mcConfigSkillLevelup}Skill Levelup", GroupOrder = 1)]
        public bool AutoTakeBothPerks { get; set; } = false;


        [SettingPropertyInteger("{=mcConfigRoleLevelupText1}Extra attribute per level", 0, 9, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigRoleLevelupDesc1}Extra attribute per level, the default is 0, and 2 is recommended")]
        [SettingPropertyGroup("{=mcConfigRoleLevelup}Role Levelup", GroupOrder = 2)]
        public int LevelupExtraAttribute { get; set; } = 0;

        [SettingPropertyInteger("{=mcConfigRoleLevelupText2}Extra focus per level", 0, 9, Order = 1, RequireRestart = false,
            HintText = "{=mcConfigRoleLevelupDesc2}Extra focus per level, the default is 0, and 2 is recommended")]
        [SettingPropertyGroup("{=mcConfigRoleLevelup}Role Levelup", GroupOrder = 2)]
        public int LevelupExtraFocus { get; set; } = 0;

        [SettingProperty("{=mcConfigRoleLevelupText3}Clan members append points", Order = 3, RequireRestart = false,
            HintText = "{=mcConfigRoleLevelupDesc3}When the unused attribute and focus points of clan members are both 0, additional level points will be added after the conversation, the default is false, and true is recommended")]
        [SettingPropertyGroup("{=mcConfigRoleLevelup}Role Levelup", GroupOrder = 2)]
        public bool CompanionAppendAttributeAndFocus { get; set; } = false;

        [SettingPropertyInteger("{=mcConfigSmithingText1}Smithing XP rate", 1, 100, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigSmithingDesc1}Skill XP rate for smithing in free build mode, the default is 1, and 10 is recommended")]
        [SettingPropertyGroup("{=mcConfigSmithing}Smithing", GroupOrder = 3)]
        public int FreeSmithingXpRate { get; set; } = 1;

        [SettingPropertyInteger("{=mcConfigSmithingText2}Research rate for new part", 1, 100, Order = 1, RequireRestart = false,
            HintText = "{=mcConfigSmithingDesc2}Research rate for unlocking new part, the default is 1, and 10 is recommended")]
        [SettingPropertyGroup("{=mcConfigSmithing}Smithing", GroupOrder = 3)]
        public int NewPartUnlockRate { get; set; } = 1;

        [SettingProperty("{=mcConfigSmithingText3}Smithing without energy", Order = 3, RequireRestart = false,
            HintText = "{=mcConfigSmithingDesc3}Smithing without cost energy, the default is false, and true is recommended")]
        [SettingPropertyGroup("{=mcConfigSmithing}Smithing", GroupOrder = 3)]
        public bool SmithingWithoutEnergyCost { get; set; } = false;


        [SettingPropertyInteger("{=mcConfigSettlementText1}Reward loyalty daily", 0, 9, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigSettlementDesc1}Clan settlements daily rewards loyalty, the default is 0, and 2 is recommended")]
        [SettingPropertyGroup("{=mcConfigSettlement}Settlement", GroupOrder = 4)]
        public int DailySettlementLoyalty { get; set; } = 0;

        [SettingPropertyInteger("{=mcConfigSettlementText2}Construction speed bonus rate", 1, 100, Order = 1, RequireRestart = false,
            HintText = "{=mcConfigSettlementDesc2}Clan settlements daily construction speed bonus rate, the default is 1x, and 10x is recommended")]
        [SettingPropertyGroup("{=mcConfigSettlement}Settlement", GroupOrder = 4)]
        public int DailySettlementBoostBonus { get; set; } = 1;


        [SettingPropertyInteger("{=mcConfigBattleText1}Gain Looted Item Rate", 0, 100, Order = 0, RequireRestart = false,
            HintText = "{=mcConfigBattleDesc1}Increase the probability of high-quality and high-value in loot reward, and the high-quality prefix is ​evenly distributed, the default is 0%, and 20% is recommended")]

        [SettingPropertyGroup("{=mcConfigBattle}Battle", GroupOrder = 5)]
        public int GainLootedItemRate { get; set; } = 0;


        [SettingProperty("{=mcConfigPregnancyText1}Disable maternal mortality", Order = 0, RequireRestart = false,
            HintText = "{=mcConfigPregnancyDesc1}Disable maternal mortality, the default is false, and true is recommended")]
        [SettingPropertyGroup("{=mcConfigPregnancy}Pregnancy", GroupOrder = 6)]
        public bool CloseMaternalMortality { get; set; } = false;

        [SettingProperty("{=mcConfigPregnancyText2}Disable stillbirth", Order = 1, RequireRestart = false,
            HintText = "{=mcConfigPregnancyDesc2}Disable stillbirth, the default is false, and true is recommended")]
        [SettingPropertyGroup("{=mcConfigPregnancy}Pregnancy", GroupOrder = 6)]
        public bool CloseStillbirth { get; set; } = false;
    }
}
