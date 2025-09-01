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
using TaleWorlds.CampaignSystem.Party;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.ObjectSystem;


namespace MultiCheats
{
    class MyBattleRewardModel: DefaultBattleRewardModel
    {
        // 战利品存在优质前缀, 则均分优质前缀概率
        public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
        {
            EquipmentElement randomItem = base.GetLootedItemFromTroop(character, targetValue);
            if (randomItem.ItemModifier != null && randomItem.ItemModifier.PriceMultiplier > 1f && (bool)GlobalSettings<MySettings>.Instance.GainLootedItemValue)
            {
                MBList<ItemModifier> _itemModifiers = new MBList<ItemModifier>();
                foreach (ItemModifier itemModifier in randomItem.Item.ItemComponent.ItemModifierGroup.ItemModifiers)
                {
                    if (itemModifier.PriceMultiplier > 1f)
                    {
                        _itemModifiers.Add(itemModifier);
                    }
                }
                if (_itemModifiers.Count > 0)
                {
                    randomItem = new EquipmentElement(randomItem.Item, _itemModifiers.GetRandomElement(), null, false);
                }
            }
            return randomItem;
        }


        // 战利品最大价值增益，关联流氓习气等级
        public override float GetExpectedLootedItemValue(CharacterObject character)
        {
            if ((bool)GlobalSettings<MySettings>.Instance.GainLootedItemValue)
            {
                return (float)(character.Level * character.Level * MobileParty.MainParty.LeaderHero.GetSkillValue(DefaultSkills.Roguery));
            }
            return base.GetExpectedLootedItemValue(character);
        }
    }
}
