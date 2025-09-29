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
using MCM.Abstractions.Base.Global;
using TaleWorlds.MountAndBlade;

namespace MB2MultiCheats
{
    // 随机工具
    internal static class MCRand
    {
        private static readonly Random rand = new Random();

        // 返回区间随机正整数
        public static int RandNum(int min, int range = 0)
        {
            return rand.Next(min, min + range + 1);
        }

        // 返回百分比概率结论
        public static bool RandBool(int rate)
        {
            return (rand.Next(0, 100) < rate);   
        }
    }

    // 日志工具
    internal static class MCLog
    {
        public static void Debug(string text, bool isTextObject = true)
        {
            Print(text, Colors.Gray, isTextObject);
        }

        public static void Info(string text, bool isTextObject = true)
        {
            Print(text, Colors.Green, isTextObject);
        }

        public static void Warn(string text, bool isTextObject = true)
        {
            Print(text, Colors.Yellow, isTextObject);
        }

        public static void Error(Exception ex)
        {
            Print("ERROR: " + ex.Message + Environment.NewLine + ex.StackTrace, Colors.Red, false);
        }

        private static void Print(string text, Color color, bool isTextObject)
        {
            InformationManager.DisplayMessage(new InformationMessage(isTextObject ? new TextObject(text).ToString() : text, color));
        }
    }
}
