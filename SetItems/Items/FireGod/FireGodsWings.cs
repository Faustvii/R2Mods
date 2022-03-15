using BepInEx.Configuration;
using Faust.SetItems.Utils;
using Faust.Shared.Compatability;
using RoR2;
using UnityEngine;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsWings : ItemBase<FireGodsWings>
    {
        public static ConfigEntry<float> BaseMovementSpeedPercentageIncrease;
        public static ConfigEntry<float> AdditionalMovementSpeedPercentageIncrease;

        public override string ItemLangTokenName => nameof(FireGodsWings);
        public override string ItemName => "Fire Gods Wings";
        public override string ItemPickupDescription => "Slight increase to movement speed";
        public override string ItemFullDescription => $"Increase movement speed by <style=cIsUtility>{MathHelpers.FloatToPercentageString(BaseMovementSpeedPercentageIncrease.Value)}</style> <style=cStack>(+{MathHelpers.FloatToPercentageString(AdditionalMovementSpeedPercentageIncrease.Value)} per stack)</style>.";
        public override string ItemLore => "NOT FOUND.";

        public override ItemTier Tier => ItemTier.Tier1;

        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand2Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand2.prefab") ?? base.ItemModel;

        public override void CreateConfig(ConfigFile config)
        {
            BaseMovementSpeedPercentageIncrease = config.Bind("Item: " + ItemLangTokenName, $"Base movement speed increase with one item", 0.1f, $"How much should {ItemName} increase movement speed with a single {ItemName}?");
            AdditionalMovementSpeedPercentageIncrease = config.Bind("Item: " + ItemLangTokenName, $"Additional movement speed increase per item", 0.05f, $"How much movement speed increase should each additional {ItemName} after the first one give?");
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RoR2Application.onLoad += OnLoadModCompat;
        }

        private void OnLoadModCompat()
        {
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddSliderToPercentageOptionsDecimal(BaseMovementSpeedPercentageIncrease, AdditionalMovementSpeedPercentageIncrease);
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            var itemCount = GetCount(self);
            if (itemCount > 0)
            {
                var attackSpeedIncreaseInPercentage = BaseMovementSpeedPercentageIncrease.Value + (AdditionalMovementSpeedPercentageIncrease.Value * (itemCount - 1));
                self.moveSpeed *= 1f + attackSpeedIncreaseInPercentage;
            }
        }
    }
}
