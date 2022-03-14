using BepInEx.Configuration;
using Faust.SetItems.Utils;
using Faust.Shared.Compatability;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsBurn : ItemBase<FireGodsBurn>
    {
        public static ConfigEntry<float> AdditionalActiviationPercentChance;
        public static ConfigEntry<float> ActiviationPercentChance;

        public override string ItemLangTokenName => nameof(FireGodsBurn);
        public override string ItemName => "Fire Gods Burn";
        public override string ItemPickupDescription => "Chance to burn on hit";
        public override string ItemFullDescription => $"Whenever you <style=cIsDamage>hit an enemy</style>, you have a <style=cIsUtility>{MathHelpers.FloatToPercentageString(ActiviationPercentChance.Value)}</style> <style=cStack>(+{MathHelpers.FloatToPercentageString(AdditionalActiviationPercentChance.Value)} per stack)</style>. chance to burn them for <style=cIsDamage>150% damage</style>";
        public override string ItemLore => "NOT FOUND.";

        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => new[] { ItemTag.Damage };


        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand4Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand4.prefab") ?? base.ItemModel;

        public override void CreateConfig(ConfigFile config)
        {
            ActiviationPercentChance = config.Bind("Item: " + ItemLangTokenName, "Activation Percent Chance", 0.25f, "What chance in percentage should the burn trigger?");
            AdditionalActiviationPercentChance = config.Bind("Item: " + ItemLangTokenName, $"Additional chance per item", 0.05f, $"How much additional chance in percentage should each {ItemName} after the first one give?");
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            RoR2Application.onLoad += OnLoadModCompat;
        }

        private void OnLoadModCompat()
        {
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddSliderToPercentageOptionsDecimal(false, ActiviationPercentChance, AdditionalActiviationPercentChance);
            }
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (damageInfo.rejected || damageInfo.procCoefficient == 0f || !NetworkServer.active || !damageInfo.attacker)
                return;

            var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            if (attackerBody)
            {
                var itemCount = GetCount(attackerBody);
                var activationChance = (ActiviationPercentChance.Value + (AdditionalActiviationPercentChance.Value * (itemCount - 1))) * 100;
                var procChance = Util.CheckRoll(activationChance * damageInfo.procCoefficient, attackerBody.master);
                if (itemCount > 0 && procChance)
                {
                    float damageMultiplier = 0.5f;
                    InflictDotInfo inflictDotInfo = new InflictDotInfo
                    {
                        attackerObject = damageInfo.attacker,
                        victimObject = victim,
                        totalDamage = new float?(damageInfo.damage),
                        damageMultiplier = damageMultiplier,
                        dotIndex = DotController.DotIndex.Burn,
                        maxStacksFromAttacker = null
                    };
                    StrengthenBurnUtils.CheckDotForUpgrade(attackerBody.inventory, ref inflictDotInfo);
                    DotController.InflictDot(ref inflictDotInfo);
                }
            }
        }
    }
}
