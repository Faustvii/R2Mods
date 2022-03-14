using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.QoLBazaar
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(LanguageAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = "QoLBazaar";
        public const string PluginVersion = "0.0.1";


        public static bool IsItemStatsModInstalled;

        public void Awake()
        {
            Log.Init(Logger);

            Hooks();
        }

        public void Configs()
        {

            //insert configs here

        }

        public void Hooks()
        {
#if DEBUG
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { }; // Allow us to connect.
#endif
            On.RoR2.BazaarController.Awake += BazaarController_Awake;
        }

        private void BazaarController_Awake(On.RoR2.BazaarController.orig_Awake orig, BazaarController self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                SpawnCauldrons();
            }
        }

        private void SpawnCauldrons()
        {
            var printerInfo = new PositionData
            {
                Position = new Vector3(-87.1f, -24f, -7.2f),
                Rotation = new Vector3(0f, 72.6f, 0f)
            };
            var printerInfo3 = new PositionData
            {
                Position = new Vector3(-82.1f, -23.7f, -5.2f),
                Rotation = new Vector3(0f, 72.6f, 0f)
            };

            var greenToWhite = GenerateCauldron(printerInfo, CostTypeIndex.GreenItem, ItemTier.Tier1);
            NetworkServer.Spawn(greenToWhite);
            var redToGreen = GenerateCauldron(printerInfo3, CostTypeIndex.RedItem, ItemTier.Tier2);
            NetworkServer.Spawn(redToGreen);
        }

        private GameObject GenerateCauldron(PositionData position, CostTypeIndex costType, ItemTier itemTier)
        {
            GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>("prefabs/networkedobjects/LunarCauldron, WhiteToGreen"), position.Position, Quaternion.identity);
            var purchaseIntercation = gameObject.GetComponent<PurchaseInteraction>();
            purchaseIntercation.Networkcost = 1;
            purchaseIntercation.costType = costType;
            gameObject.transform.eulerAngles = position.Rotation;
            var behavior = gameObject.GetComponent<ShopTerminalBehavior>();
            if (behavior)
            {
                behavior.itemTier = itemTier;
            }
            return gameObject;
        }

        public struct PositionData
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }
    }

}
