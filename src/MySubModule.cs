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
using TaleWorlds.MountAndBlade;

using HarmonyLib;
using Bannerlord.UIExtenderEx;

namespace MB2MultiCheats
{
    internal class MySubModule : MBSubModuleBase
    {
        public static readonly string ModuleName = typeof(MySubModule).Namespace;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                // 初始化顺序: UIExtender -> Harmony
                UIExtender extender = UIExtender.Create(ModuleName);
                extender.Register(typeof(MySubModule).Assembly);
                extender.Enable();

                new Harmony(ModuleName).PatchAll();
            }
            catch (Exception ex)
            {
                MCLog.Error(ex);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                gameStarterObject.AddModel(new MyCharacterDevelopmentModel());
                gameStarterObject.AddModel(new MySmithingModel());
                gameStarterObject.AddModel(new MyPregnancyModel());
                gameStarterObject.AddModel(new MyBuildingConstructionModel());
                gameStarterObject.AddModel(new MyPartySizeLimitModel());
                gameStarterObject.AddModel(new MyPrisonerRecruitmentCalculationModel());
                gameStarterObject.AddModel(new MyBattleRewardModel());
                gameStarterObject.AddModel(new MyDiplomacyModel());

                if (gameStarterObject is CampaignGameStarter starter)
                {
                    starter.AddBehavior(new MyBehaviors());
                    starter.AddBehavior(new MyTalkBehavior());
                }
            }
            catch (Exception ex)
            {
                MCLog.Error(ex);
            }
        }
    }
}
