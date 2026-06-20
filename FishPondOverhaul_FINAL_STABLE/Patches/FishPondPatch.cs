using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;
using System;

namespace FishPondOverhaul.Patches
{
    public class FishPondPatch
    {
        private static Harmony harmony;
        private static ModEntry mod;

        public static void Apply(ModEntry m)
        {
            mod = m;
            harmony = new Harmony("chatgpt.fishpond.final");

            // 拦截鱼塘每天结算产出的方法
            var method = AccessTools.Method(typeof(FishPond), nameof(FishPond.DayUpdate));

            if (method == null)
                return;

            // 使用 Postfix（后置补丁），在游戏计算完池塘产出后，立刻将其数量翻倍
            harmony.Patch(method,
                postfix: new HarmonyMethod(typeof(FishPondPatch), nameof(Postfix)));
        }

        public static void Postfix(FishPond __instance)
        {
            // 如果产出箱为空，直接跳过
            if (__instance.output.Value == null) return;

            int fishCount = __instance.fishCount.Value;
            if (fishCount <= 0) fishCount = 1;

            double mult = ModEntry.Config.FishMultiplier;
            
            // 最终翻倍系数 = 鱼的数量 * 用户在菜单里设置的倍率
            double finalMultiplier = fishCount * mult;

            if (finalMultiplier <= 1.0) return;

            // 遍历鱼塘产出箱里的所有物品，将其堆叠数量乘以系数
            foreach (var item in __instance.output.Value.Items)
            {
                if (item != null)
                {
                    int bonus = (int)Math.Round(item.Stack * (finalMultiplier - 1.0));
                    if (bonus > 0)
                    {
                        item.Stack += bonus;
                    }
                }
            }
        }
    }
}