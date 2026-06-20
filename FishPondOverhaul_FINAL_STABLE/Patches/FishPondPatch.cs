using HarmonyLib;
using StardewValley.Buildings;

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

            var method = AccessTools.Method(typeof(FishPond), "getFishProduce");

            if (method == null)
                return;

            harmony.Patch(method,
                postfix: new HarmonyMethod(typeof(FishPondPatch), nameof(Postfix)));
        }

        public static void Postfix(FishPond __instance, ref int __result)
        {
            int count = __instance.fishCount.Value;
            if (count <= 0) count = 1;

            double mult = ModEntry.Config.FishMultiplier;

            __result = (int)(__result * count * mult);
        }
    }
}
