using BepInEx.Configuration;
using Faust.SetItems.Utils;
using Faust.Shared.Compatability;
using RoR2;
using System.Linq;
using UnityEngine;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsShield : ItemBase<FireGodsShield>
    {
        public override string ItemLangTokenName => nameof(FireGodsShield);
        public override string ItemName => "Fire Gods Shield";
        public override string ItemPickupDescription => "Reduces the duration of burning debuffs";
        public override string ItemFullDescription => $"Reduces the duration of burning debuffs on yourself, by <style=cIsUtility>{MathHelpers.FloatToPercentageString(DurationPercent.Value)}</style> to a minimum of 1 tick <style=cStack>(+{MathHelpers.FloatToPercentageString(AdditionalPercent.Value)} per stack)</style>.";
        public override string ItemLore => "It's a wand that protects you from fire at least a little bit.";

        public static ConfigEntry<float> AdditionalPercent;
        public static ConfigEntry<float> DurationPercent;


        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => new[] { ItemTag.Utility };

        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand3Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand3.prefab") ?? base.ItemModel;

        private static DotController.DotIndex[] BurnDots => new[]{
            DotController.DotIndex.Burn,
            DotController.DotIndex.StrongerBurn,
            DotController.DotIndex.PercentBurn,
        };

        public override void CreateConfig(ConfigFile config)
        {
            DurationPercent = config.Bind("Item: " + ItemLangTokenName, "Reduce debuff duration by percentage", 0.2f, "What percentage should the debuffs duration be reduced by?");
            AdditionalPercent = config.Bind("Item: " + ItemLangTokenName, $"Additional reduction percentage per item", 0.1f, $"What additional reduction percentage should each {ItemName} after the first one give?");
        }

        public override void Hooks()
        {
            On.RoR2.DotController.OnDotStackAddedServer += DotController_OnDotStackAddedServer;
            RoR2Application.onLoad += OnLoadModCompat;
        }

        private void OnLoadModCompat()
        {
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddSliderToPercentageOptionsDecimal(DurationPercent, AdditionalPercent);
            }
        }

        private void DotController_OnDotStackAddedServer(On.RoR2.DotController.orig_OnDotStackAddedServer orig, DotController self, object dotStack)
        {
            orig(self, dotStack);
            var dot = dotStack as DotController.DotStack;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            if (self && self.victimBody && BurnDots.Contains(dot.dotIndex))
            {
                dot.timer = GetModifiedDuration(self.victimBody, dot.timer);
            }
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
        }

        public float GetModifiedDuration(CharacterBody body, float duration)
        {
            var inventory = body.inventory;
            if (inventory)
            {
                int itemCount = GetCount(body);
                if (itemCount > 0)
                {
                    var reductionPercentage = Mathf.Min(1f - (DurationPercent.Value + (AdditionalPercent.Value * (itemCount - 1))), 1f);
                    duration = Mathf.Min(Mathf.Max(duration * reductionPercentage, 0f), duration);
                }
            }
            return duration;
        }
    }
}
