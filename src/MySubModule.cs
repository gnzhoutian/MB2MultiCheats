using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;

namespace MultiCheats
{
    class MySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("MultiCheats").PatchAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                InformationManager.DisplayMessage(new InformationMessage(ex.Message));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                gameStarterObject.AddModel(new MySmithingModel());
                gameStarterObject.AddModel(new MyCharacterDevelopmentModel());

                if (gameStarterObject is CampaignGameStarter starter)
                {
                    starter.AddBehavior(new MyBehaviors());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                InformationManager.DisplayMessage(new InformationMessage(ex.Message));
            }
        }
    }
}
