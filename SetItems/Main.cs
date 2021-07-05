using BepInEx;
using Faust.SetItems.Items;
using Faust.SetItems.SoftDependencies;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Faust.SetItems
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(ItemStatsSoftDependency.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(LanguageAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = "SetItems";
        public const string PluginVersion = "0.0.1";

        public List<ItemBase> Items = new List<ItemBase>();
        public List<ItemBaseSet> ItemSets = new List<ItemBaseSet>();

        public static bool IsItemStatsModInstalled;

        public void Awake()
        {
            Log.Init(Logger);
            IsItemStatsModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ItemStatsSoftDependency.PluginGUID);

            Assets.PopulateAssets();

            //Item Initialization
            var itemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            Log.LogInfo("----------------------ITEMS--------------------");

            foreach (var itemType in itemTypes)
            {
                var item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);

                    Log.LogInfo("Item: " + item.ItemName + " Initialized!");
                }
            }

            //Item Set Initialization
            var itemSets = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBaseSet)));

            Log.LogInfo("-------------------ITEM SETS-------------------");

            //ItemSetController.Init();
            foreach (var itemType in itemSets)
            {
                var itemSet = (ItemBaseSet)System.Activator.CreateInstance(itemType);
                ItemSets.Add(itemSet);
                //if (ValidateItem(item, Items))
                //{
                    itemSet.Init(Config);
                    Log.LogInfo("Item Set: " + itemSet.SetName + " Initialized!");
                //}
            }

            //runs hooks that are seperate from all additions (i.e, if you need to call something when the game runs or at special times)
            Hooks();

            //initializes our contentpack to be added to the game
            new CustomContentPack().Init(Items.Select(x => x.ItemDef).ToArray(), Assets.effectDefs.ToArray(), new[] { Assets.fireAuraCirclePrefab });

        }

        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind("Item: " + item.ItemLangTokenName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind("Item: " + item.ItemLangTokenName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;

            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        public void Configs()
        {

            //insert configs here

        }

        public void Hooks()
        {
            On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { }; // Allow us to connect.
        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
            //This if statement checks if the player has currently pressed F3.
            if (Input.GetKeyDown(KeyCode.F3))
            {
                //Get the player body to use a position:	
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then drop our defined item in front of the player.

                Log.LogInfo($"Player pressed F3. Spawning our custom item at coordinates {transform.position}");
                for (var i = 0; i < Items.Count; i++)
                {
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(Items[i].ItemDef.itemIndex), transform.position, transform.forward * (20f + (i * 3)));
                }
            }
        }

    }

}
