using BepInEx.Configuration;
using Faust.SetItems.Utils;
using Faust.Shared.Compatability;
using R2API;
using RoR2;
using UnityEngine;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsHaste : ItemBase<FireGodsHaste>
    {
        public static ConfigEntry<float> BaseAttackSpeedPercentageIncrease;
        public static ConfigEntry<float> AdditionalAttackSpeedPercentageIncrease;

        public override string ItemLangTokenName => nameof(FireGodsHaste);
        public override string ItemName => "Fire Gods Haste";
        public override string ItemPickupDescription => "Slight increase to attack speed";
        public override string ItemFullDescription => $"Increase attack speed by <style=cIsUtility>{MathHelpers.FloatToPercentageString(BaseAttackSpeedPercentageIncrease.Value)}</style> <style=cStack>(+{MathHelpers.FloatToPercentageString(AdditionalAttackSpeedPercentageIncrease.Value)} per stack)</style>.";
        public override string ItemLore => "NOT FOUND.";

        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => new[] { ItemTag.Utility, ItemTag.OnKillEffect };

        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand1Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand1.prefab") ?? base.ItemModel;

        public override void CreateConfig(ConfigFile config)
        {
            BaseAttackSpeedPercentageIncrease = config.Bind("Item: " + ItemLangTokenName, $"Base attack speed increase with one item", 0.10f, $"How much should {ItemName} increase attack speed with a single {ItemName}?");
            AdditionalAttackSpeedPercentageIncrease = config.Bind("Item: " + ItemLangTokenName, $"Additional attack speed increase per item", 0.05f, $"How much attack speed increase should each additional {ItemName} after the first one give?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            var rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
               {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.1348F, -0.0421F, 0.024F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(1F, 1F, 1F)
                },
            });
            rules.Add("mdlToolbot", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "ThighL",
                localPos = new Vector3(0.2649F, -0.0965F, 0.9197F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(5F, 5F, 5F)
            });
            rules.Add("mdlCroco", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "ThighL",
                localPos = new Vector3(1.4406F, 0.2491F, -0.0716F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(5F, 5F, 5F)
            });

            return rules;
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            RoR2Application.onLoad += OnLoadModCompat;
        }

        private void OnLoadModCompat()
        {
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddSliderToPercentageOptionsDecimal(false, BaseAttackSpeedPercentageIncrease, AdditionalAttackSpeedPercentageIncrease);
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = GetCount(sender);
            if (itemCount > 0)
            {
                var attackSpeedIncreaseInPercentage = BaseAttackSpeedPercentageIncrease.Value + (AdditionalAttackSpeedPercentageIncrease.Value  * (itemCount - 1));
                args.attackSpeedMultAdd += attackSpeedIncreaseInPercentage;
            }
        }
    }
}
