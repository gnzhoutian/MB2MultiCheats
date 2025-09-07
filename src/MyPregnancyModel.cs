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


namespace MB2MultiCheats
{
    internal class MyPregnancyModel: DefaultPregnancyModel
    {
        public override float MaternalMortalityProbabilityInLabor
        {
            get
            {
                if ((bool)GlobalSettings<MySettings>.Instance.CloseMaternalMortality)
                {
                    return 0f;
                }
                return 0.015f;
                    
            }
        }

        public override float StillbirthProbability
        {
            get
            {
                if ((bool)GlobalSettings<MySettings>.Instance.CloseStillbirth)
                {
                    return 0f;
                }
                return 0.01f;
            }
        }
    }
}
