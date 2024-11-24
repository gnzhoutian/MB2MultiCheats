using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;


namespace MultiCheats.Patches
{
    // 锻造显示价值 -> Add the value to the property list
    [HarmonyPatch(typeof(WeaponDesignVM), "RefreshStats")]
    internal class PatchRefreshStats
    {
        private static AccessTools.FieldRef<WeaponDesignVM, MBBindingList<CraftingListPropertyItem>> primaryPropertyListRef = AccessTools.FieldRefAccess<WeaponDesignVM, MBBindingList<CraftingListPropertyItem>>("_primaryPropertyList");
        private static AccessTools.FieldRef<WeaponDesignVM, Crafting> craftingRef = AccessTools.FieldRefAccess<WeaponDesignVM, Crafting>("_crafting");
        private static void Postfix(WeaponDesignVM __instance)
        {
            ItemObject weapon = craftingRef(__instance).GetCurrentCraftedItemObject(false);
            EquipmentElement equipment = new EquipmentElement(weapon);
            int price = Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(equipment, Campaign.Current.MainParty, null, true, 0, 0, 0);
            CraftingListPropertyItem valueItem = new CraftingListPropertyItem(new TextObject("{=mcMainPatchWeaponDesignValue}Value: ", null), 99999f, (float)price, 0f, CraftingTemplate.CraftingStatTypes.NumStatTypes, false);
            valueItem.IsValidForUsage = true;
            primaryPropertyListRef(__instance).Add(valueItem);
        }
    }

    // 锻造显示价值 -> Prevent the value from being in the result screen
    [HarmonyPatch(typeof(WeaponDesignVM), "UpdateResultPropertyList")]
    internal class PatchUpdateResult
    {
        private static AccessTools.FieldRef<WeaponDesignVM, MBBindingList<WeaponDesignResultPropertyItemVM>> designResultRef = AccessTools.FieldRefAccess<WeaponDesignVM, MBBindingList<WeaponDesignResultPropertyItemVM>>("_designResultPropertyList");
        private static void Postfix(WeaponDesignVM __instance)
        {
            designResultRef(__instance).RemoveAt(designResultRef(__instance).Count - 1);
        }
    }
}
