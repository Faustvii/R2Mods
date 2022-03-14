using BepInEx;
using BepInEx.Configuration;
using Faust.Shared;
using Faust.Shared.Compatability;
using RoR2;
using System.Diagnostics;
using UnityEngine.Networking;

namespace Faust.LunarCoinShare
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(RiskOfOptionsCompat.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class LunarCoinShare : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = nameof(LunarCoinShare);
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<bool> ShareEnabled;

        public void Awake()
        {
            var watch = Stopwatch.StartNew();
            Log.Init(Logger);

            // Configuration
            ShareEnabled = Config.Bind("Lunar Share", "Enabled", true, "If the mod should share lunar coin pickups or not.");

            // Check for softdependencies
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.SetModDescription("When a player picks up a lunar coin, everyone in the game gets one.");
                RiskOfOptionsCompat.AddCheckboxOptions(restartRequired: false, ShareEnabled);
            }

            // Hooks
            On.RoR2.GenericPickupController.OnInteractionBegin += GenericPickupController_OnInteractionBegin;

            watch.Stop();
            Log.LogInfo($"{nameof(Awake)} done in {watch.ElapsedMilliseconds} ms.");
        }

        private void GenericPickupController_OnInteractionBegin(On.RoR2.GenericPickupController.orig_OnInteractionBegin orig, GenericPickupController self, Interactor activator)
        {
            var isLunarCoin = self.pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex);
            if (isLunarCoin && ShareEnabled.Value)
            {
                if (!NetworkServer.active)
                    return;

                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    player.networkUser.AwardLunarCoins(1);
                }
                Destroy(self.gameObject);
            }
            else
            {
                orig(self, activator);
            }
        }
    }
}
