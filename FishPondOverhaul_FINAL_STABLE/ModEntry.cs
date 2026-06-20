using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace FishPondOverhaul
{
    public class ModEntry : Mod
    {
        public static ModConfig Config = new ModConfig();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += (_,__) => ApplyPatches();

            Monitor.Log("Fish Pond Overhaul FINAL STABLE loaded", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            TrySetupGMCM();
            ApplyPatches();
        }

        private void TrySetupGMCM()
        {
            try
            {
                var api = Helper.ModRegistry.GetApi<dynamic>("spacechase0.GenericModConfigMenu");
                if (api == null) return;

                api.Register(ModManifest,
                    () => Config = new ModConfig(),
                    () => Helper.WriteConfig(Config));

                api.AddNumberOption(ModManifest,
                    () => "Fish Multiplier",
                    () => Config.FishMultiplier,
                    v => Config.FishMultiplier = v,
                    1, 10);

                Monitor.Log("GMCM loaded", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log("GMCM optional skipped: " + ex.Message, LogLevel.Warn);
            }
        }

        private void ApplyPatches()
        {
            Patches.FishPondPatch.Apply(this);
        }
    }

    public class ModConfig
    {
        public double FishMultiplier { get; set; } = 1.0;
    }
}
