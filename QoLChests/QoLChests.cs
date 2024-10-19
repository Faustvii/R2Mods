using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BepInEx;
using Faust.QoLChests.Configs;
using Faust.Shared;
using Faust.Shared.Compatability;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Faust.QoLChests
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(
        RiskOfOptionsCompat.PluginGUID,
        BepInDependency.DependencyFlags.SoftDependency
    )]
    public class QoLChests : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = nameof(QoLChests);
        public const string PluginVersion = "1.2.0";

        //Configuration
        public ModConfig Configuration { get; private set; }

        private List<GameObject> TrackedSceneInteractables { get; set; } = [];
        private List<GameObject> TrackedInteractables { get; set; } = [];
        private List<GameObject> InteractablesToHighlight { get; set; } = [];

        protected void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            var stopwatch = Stopwatch.StartNew();
            Log.Init(Logger);
            Configuration = new ModConfig(Config);

            if (StarStorm2Compat.IsInstalled)
            {
                InteractableRegistry.Register("ShockDroneBroken", InteractableCategory.Drone);
                InteractableRegistry.Register("CloneDroneBroken", InteractableCategory.Drone);
                InteractableRegistry.Register("DroneTablePrefab", InteractableCategory.Duplicator);
            }

            On.EntityStates.Barrel.Opened.OnEnter += BarrelOpened;
            On.RoR2.MultiShopController.OnPurchase += MultiShopController_OnPurchase;
            On.RoR2.RouletteChestController.Opened.OnEnter += Roulette_Opened;
            On.RoR2.DelusionChestController.ResetChestForDelusion +=
                On_DelusionChestController_ResetChestForDelusion;

            On.RoR2.InteractableSpawnCard.Spawn += TrackSpawnedInteractable;
            On.RoR2.SceneDirector.PopulateScene += (orig, self) =>
            {
                TrackedSceneInteractables.Clear();
                InteractablesToHighlight.Clear();
                orig(self);
                ConfigureHighlights();
                Log.LogInfo(
                    $"SceneDirector.PopulateScene - {InteractablesToHighlight.Count} interactables highlighted"
                );
            };

            RoR2Application.onLoad += LoadHighlightableResources;
            Config.SettingChanged += (sender, args) =>
            {
                ConfigureHighlights();
            };

            stopwatch.Stop();

            Log.LogDebug($"{nameof(Awake)} took {stopwatch.ElapsedMilliseconds}ms");
        }

        public void Update() { }

        private void TrackSpawnedInteractable(
            On.RoR2.InteractableSpawnCard.orig_Spawn orig,
            InteractableSpawnCard self,
            Vector3 position,
            Quaternion rotation,
            DirectorSpawnRequest directorSpawnRequest,
            ref SpawnCard.SpawnResult result
        )
        {
            orig.Invoke(self, position, rotation, directorSpawnRequest, ref result);
            var spawned = result.spawnedInstance;
            if (!spawned)
            {
                return;
            }

            var highlightCategoryMarker =
                spawned.GetComponent<InteractableHighlightCategoryMarker>();
            if (highlightCategoryMarker)
            {
                TrackedSceneInteractables.Add(spawned);
            }
            else if (InteractableRegistry.IsRegistered(spawned.name, out var category))
            {
                Log.LogInfo(
                    $"InteractableSpawnCard.Spawn - {spawned.name} (Modded interactable) registered as {category}"
                );
                spawned.AddComponent<InteractableHighlightCategoryMarker>().SetCategory(category);

                TrackedSceneInteractables.Add(spawned);
                if (Configuration.IsCategoryEnabled(category))
                {
                    InteractablesToHighlight.Add(spawned);
                    AddHighlight(spawned);
                }
            }
        }

        private void LoadHighlightableResources()
        {
            // Load highlightable resources
            AddResourcesToHighlights(Constants.ChestResourcesPaths);
            AddResourcesToHighlights(Constants.LockboxResourcesPaths);
            AddResourcesToHighlights(Constants.StealthedChestResourcePaths);
            AddResourcesToHighlights(Constants.ShopResourcePaths);
            AddResourcesToHighlights(Constants.ScrapperResourcePaths);
            AddResourcesToHighlights(Constants.DuplicatorResourcesPaths);
            AddResourcesToHighlights(Constants.DroneResourcesPaths);
            AddResourcesToHighlights(Constants.TurrentResourcePaths);
            AddResourcesToHighlights(Constants.ArtifactOfDevotionResourcePaths);
        }

        private void ConfigureHighlights()
        {
            // Mark interactables for highlighting based on config
            foreach (var interactable in InteractablesToHighlight)
            {
                RemoveHighlight(interactable);
            }
            InteractablesToHighlight.Clear();
            var trackedInteractables = TrackedSceneInteractables
                .Concat(TrackedInteractables)
                .ToArray();

            foreach (var interactable in trackedInteractables)
            {
                var highlightCategoryMarker =
                    interactable.GetComponent<InteractableHighlightCategoryMarker>();
                if (!highlightCategoryMarker)
                {
                    Log.LogWarning(
                        $"Interactable {interactable.name} does not have a HighlightCategoryMarker - why are we tracking it?"
                    );
                    continue;
                }
                if (Configuration.IsCategoryEnabled(highlightCategoryMarker.Category))
                    InteractablesToHighlight.Add(interactable);
            }

            Log.LogDebug(
                $"Configuring highlights - {trackedInteractables.Length} tracked interactables out of which {InteractablesToHighlight.Count} will be highlighted"
            );

            // Add highlights to tracked resources
            AddHighlights();
        }

        private void BarrelOpened(
            On.EntityStates.Barrel.Opened.orig_OnEnter orig,
            EntityStates.Barrel.Opened self
        )
        {
            orig.Invoke(self);
            if (Configuration.HideEmptyChests.Value)
            {
                HideOrFade(
                    self.outer.gameObject,
                    self.outer.commonComponents.modelLocator.modelTransform.gameObject
                );
            }
            else if (Configuration.RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private void On_DelusionChestController_ResetChestForDelusion(
            On.RoR2.DelusionChestController.orig_ResetChestForDelusion orig,
            DelusionChestController self
        )
        {
            orig.Invoke(self);

            var modelLocator = self.gameObject.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                var modelTransformHide =
                    modelLocator.modelTransform.gameObject.GetComponent<HideWithDelay>();
                var modelTransformFade =
                    modelLocator.modelTransform.gameObject.GetComponent<FadeWithDelay>();
                if (modelTransformHide)
                    Destroy(modelTransformHide);
                if (modelTransformFade)
                    Destroy(modelTransformFade);
            }
            var hide = self.gameObject.GetComponent<HideWithDelay>();
            if (hide)
                Destroy(hide);
            var fade = self.gameObject.GetComponent<FadeWithDelay>();
            if (fade)
                Destroy(fade);

            if (Configuration.HighlightChests.Value)
            {
                AddHighlight(self.gameObject);
            }
        }

        private void MultiShopController_OnPurchase(
            On.RoR2.MultiShopController.orig_OnPurchase orig,
            MultiShopController self,
            Interactor interactor,
            PurchaseInteraction purchaseInteraction
        )
        {
            orig.Invoke(self, interactor, purchaseInteraction);
            if (self.Networkavailable)
                return;

            var terminalObjects = self
                .terminalGameObjects.Select(x => x)
                .Concat([self.gameObject])
                .ToArray();

            if (Configuration.HideUsedShops.Value)
            {
                HideOrFade(terminalObjects);
            }
            else if (Configuration.RemoveHighlightFromUsed.Value)
            {
                foreach (var terminalObj in terminalObjects)
                {
                    RemoveHighlight(terminalObj);
                }
            }
        }

        private void Roulette_Opened(
            On.RoR2.RouletteChestController.Opened.orig_OnEnter orig,
            EntityStates.EntityState self
        )
        {
            orig.Invoke(self);
            if (Configuration.HideEmptyChests.Value)
            {
                HideOrFade(self.outer.gameObject);
            }
            else if (Configuration.RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private void HideOrFade(params GameObject[] gameObjects)
        {
            if (Configuration.FadeInsteadOfHide.Value)
            {
                foreach (var obj in gameObjects)
                {
                    obj.AddComponent<FadeWithDelay>()
                        .SetDelay(Configuration.HideTime.Value)
                        .DisableRendererAfterDelay();
                }
                return;
            }

            foreach (var obj in gameObjects)
            {
                obj.AddComponent<HideWithDelay>()
                    .SetDelay(Configuration.HideTime.Value)
                    .DisableRendererAfterDelay();
            }
        }

        private void AddResourcesToHighlights(HighlightableResource[] resources)
        {
            foreach (var resource in resources)
            {
                var addressable = Addressables
                    .LoadAssetAsync<GameObject>(resource.ResourcePath)
                    .WaitForCompletion();
                if (addressable)
                {
                    addressable
                        .AddComponent<InteractableHighlightCategoryMarker>()
                        .SetCategory(resource.Category);
                    TrackedInteractables.Add(addressable);
                }
            }
        }

        private static void RemoveHighlight(GameObject self)
        {
            if (!self)
                return;
            var highlight = self.GetComponent<Highlight>();
            if (highlight)
            {
                highlight.isOn = false;
                highlight.enabled = false;
            }
            else
            {
                Log.LogDebug($"GameObject {self.name} does not have a Highlight component");
            }
        }

        private void AddHighlights()
        {
            foreach (var gameObject in InteractablesToHighlight)
            {
                AddHighlight(gameObject);
            }
        }

        private void AddHighlight(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            var highlight = gameObject.GetComponent<Highlight>();
            if (!highlight)
            {
                return;
            }

            if (Configuration.RemoveHighlightFromUsed.Value)
            {
                var multiShop = gameObject.GetComponent<MultiShopController>();
                if (multiShop && !multiShop.Networkavailable)
                {
                    return;
                }

                var barrel = gameObject.GetComponent<BarrelInteraction>();
                if (barrel && barrel.Networkopened)
                {
                    return;
                }

                var rouletteChestController = gameObject.GetComponent<RouletteChestController>();
                if (rouletteChestController && !rouletteChestController.enabled)
                {
                    return;
                }
            }

            var highlightCategory = gameObject.GetComponent<InteractableHighlightCategoryMarker>();
            if (!highlightCategory)
            {
                Log.LogWarning(
                    $"GameObject {gameObject.name} does not have a HighlightCategoryMarker - why are we trying to highlight it?"
                );
                return;
            }
            var highlightConfigColor = Configuration
                .GetCategoryHighlightColorConfig(highlightCategory.Category)
                .Value;

            highlight.isOn = true;
            highlight.enabled = true;
            highlight.highlightColor = Highlight.HighlightColor.custom;
            highlight.CustomColor = Constants.GetColor(highlightConfigColor);
        }
    }
}
