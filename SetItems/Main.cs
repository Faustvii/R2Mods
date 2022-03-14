using BepInEx;
using Faust.SetItems.Items;
using Faust.SetItems.SoftDependencies;
using Faust.Shared;
using Faust.Shared.Compatability;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Faust.SetItems.GenericDamageEvents;

namespace Faust.SetItems
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(ItemStatsSoftDependency.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(BetterUICompat.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ItemAPI), nameof(PrefabAPI), nameof(RecalculateStatsAPI))]
    public class Main : BaseUnityPlugin
    {

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = "SetItems";
        public const string PluginVersion = "0.0.3";

        public List<ItemBase> Items = new List<ItemBase>();
        public List<ItemBaseSet> ItemSets = new List<ItemBaseSet>();

        public static bool IsItemStatsModInstalled;
        public static bool IsRiskOfOptionsModInstalled;

        public void Awake()
        {
            var watch = Stopwatch.StartNew();
            Log.Init(Logger);

            Configs();

            CheckSoftDependencies();

            Assets.PopulateAssets();

            RegisterItems();

            Buffs.Buffs.RegisterBuffs();

            //RoR2Application.onLoad += Thing;
            Hooks();

            new CustomContentPack().Init(Items.Select(x => x.ItemDef).ToArray(), Assets.effectDefs.ToArray(), new[] { Assets.fireAuraCirclePrefab }, Buffs.Buffs.BuffDefs.ToArray());
            watch.Stop();
            Log.LogInfo($"{nameof(Awake)} done in {watch.ElapsedMilliseconds} ms");

        }

        private static void CheckSoftDependencies()
        {
            IsItemStatsModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ItemStatsSoftDependency.PluginGUID);
            IsRiskOfOptionsModInstalled = RiskOfOptionsCompat.IsInstalled;

            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.SetModDescription("Adds set items to the game, combine a full set for additional effects.");
            }
        }

        private void RegisterItems()
        {
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
        }

        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind("Item: " + item.ItemLangTokenName, "Enable Item?", true, "Should this item appear in runs?");
            var aiBlacklist = Config.Bind("Item: " + item.ItemLangTokenName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?");
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddCheckboxOptions(true, enabled, aiBlacklist);
            }
            if (enabled.Value)
            {
                itemList.Add(item);
                if (aiBlacklist.Value)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled.Value;
        }

        public void Configs()
        {

            //insert configs here

        }

        public void Hooks()
        {
#if DEBUG
            On.RoR2.Networking.NetworkManagerSystem.OnClientConnect += (self, user, t) => { }; // Allow us to connect.
#endif

            On.RoR2.HealthComponent.Awake += (orig, self) =>
            {
                self.gameObject.AddComponent<DamageEvents>();
                orig(self);
            };

        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
#if !DEBUG
            return; 
#endif
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


            //if (Input.GetKeyDown(KeyCode.F4))
            //{
            //    var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            //    body.AddBuff(Buffs.Buffs.FireGodsWrath);
            //    body.AddBuff(Buffs.Buffs.Icon1);
            //    body.AddBuff(Buffs.Buffs.Icon2);
            //    body.AddBuff(Buffs.Buffs.Icon3);
            //    body.AddBuff(Buffs.Buffs.Icon4);
            //    body.AddBuff(Buffs.Buffs.Icon5);
            //}
        }





        private void Thing()
        {
            var assetlazy = Addressables.LoadAssetAsync<UnityEngine.Object>("RoR2/Base/Huntress/Skins.Huntress.Alt1.asset").WaitForCompletion();
            Logger.LogError(assetlazy);

            var sb = new StringBuilder();

            var i = 0;

            using (var table = new Table(sb))
            {

                using (var headerRow = table.AddRow())
                {
                    headerRow.AddCell("Key");
                    headerRow.AddCell("Type");
                }

                foreach (var item in Addressables.ResourceLocators)
                {
                    foreach (var key in item.Keys)
                    {
                        try
                        {
                            i++;
                            if (key.ToString().Contains("bundle") || !key.ToString().Contains(".png"))
                                continue;

                            if (!key.ToString().Contains("."))
                            {
                                var digitCount = 0;
                                foreach (var c in key.ToString())
                                {
                                    if (char.IsDigit(c))
                                    {
                                        digitCount++;
                                        if (digitCount > 5)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (digitCount > 5)
                                {
                                    continue;
                                }

                                using (var row = table.AddRow())
                                {
                                    row.AddCell(key.ToString());
                                    row.AddCell(key.ToString());
                                }
                            }
                            else
                            {
                                var digitCount = 0;
                                foreach (var c in key.ToString())
                                {
                                    if (char.IsDigit(c))
                                    {
                                        digitCount++;
                                        if (digitCount > 5)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (digitCount > 5)
                                {
                                    continue;
                                }

                                Logger.LogError(i + " | " + key.ToString());
                                var asset = Addressables.LoadAssetAsync<UnityEngine.Object>(key).WaitForCompletion();
                                using (var row = table.AddRow())
                                {
                                    row.AddCell(key.ToString());
                                    row.AddCell(asset.GetType().ToString());
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }

            Logger.LogError(sb.ToString());
        }

        public class Table : IDisposable
        {
            private StringBuilder _sb;

            public Table(StringBuilder sb, string id = "default", string classValue = "")
            {
                _sb = sb;
                _sb.Append($"<table id=\"{id}\" class=\"{classValue}\">\n");
            }

            public void Dispose()
            {
                _sb.Append("</table>");
            }

            public Row AddRow()
            {
                return new Row(_sb);
            }

            public Row AddHeaderRow()
            {
                return new Row(_sb, true);
            }

            public void StartTableBody()
            {
                _sb.Append("<tbody>");

            }

            public void EndTableBody()
            {
                _sb.Append("</tbody>");

            }
        }

        public class Row : IDisposable
        {
            private StringBuilder _sb;
            private bool _isHeader;
            public Row(StringBuilder sb, bool isHeader = false)
            {
                _sb = sb;
                _isHeader = isHeader;
                if (_isHeader)
                {
                    _sb.Append("<thead>\n");
                }
                _sb.Append("\t<tr>\n");
            }

            public void Dispose()
            {
                _sb.Append("\t</tr>\n");
                if (_isHeader)
                {
                    _sb.Append("</thead>\n");
                }
            }

            public void AddCell(string innerText)
            {
                _sb.Append("\t\t<td>\n");
                _sb.Append("\t\t\t" + innerText);
                _sb.Append("\t\t</td>\n");
            }
        }
    }

}
