using BepInEx.Configuration;
using Faust.SetItems.SoftDependencies;
using Faust.SetItems.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsWand1 : ItemBase<FireGodsWand1>
    {
        public static ConfigOption<float> BaseDurationOfBuffInSeconds;
        public static ConfigOption<float> AdditionalDurationOfBuffInSeconds;
        public static ConfigOption<float> ActiviationPercentChance;

        public override string ItemLangTokenName => nameof(FireGodsWand1);
        public override string ItemName => "Fire Gods Wand 1";
        public override string ItemPickupDescription => "Chance to cloak on kill";
        public override string ItemFullDescription => $"Whenever you <style=cIsDamage>kill an enemy</style>, you have a <style=cIsUtility>{MathHelpers.FloatToPercentageString(ActiviationPercentChance)}</style> chance to cloak for <style=cIsUtility>{BaseDurationOfBuffInSeconds}s</style> <style=cStack>(+{AdditionalDurationOfBuffInSeconds}s per stack)</style>.";
        public override string ItemLore => "Those who visit in the night are either praying for a favour, or preying on a neighbour.";

        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => new[] { ItemTag.Utility, ItemTag.OnKillEffect };

        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand1Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand1.prefab") ?? base.ItemModel;

        public override void CreateConfig(ConfigFile config)
        {
            BaseDurationOfBuffInSeconds = config.ActiveBind("Item: " + ItemLangTokenName, $"Base Duration of Buff with One {ItemLangTokenName}", 4f, $"How many seconds should {ItemName} buff last with a single {ItemName}?");
            ActiviationPercentChance = config.ActiveBind("Item: " + ItemLangTokenName, "Activation Percent Chance", 0.2f, "What chance in percentage should the cloaking ability trigger?");
            AdditionalDurationOfBuffInSeconds = config.ActiveBind("Item: " + ItemLangTokenName, $"Additional Duration of Buff per {ItemLangTokenName} Stack", 0.5f, $"How many additional seconds of buff should each {ItemName} after the first one give?");
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
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            if (Main.IsItemStatsModInstalled)
            {
                RoR2Application.onLoad += ItemStatsModCompat;
            }
        }

        private void ItemStatsModCompat()
        {
            ItemStatsSoftDependency.CreateFireGodsWand1StatDef();
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            //If a character was killed by the world, we shouldn't do anything.
            if (!report.attacker || !report.attackerBody || !NetworkServer.active)
                return;

            var attacker = report.attackerBody;

            if (attacker.inventory)
            {
                var itemCount = GetCount(attacker);
                if (itemCount > 0 && Util.CheckRoll(ActiviationPercentChance * 100, attacker.master))
                {
                    var stackTime = BaseDurationOfBuffInSeconds + (AdditionalDurationOfBuffInSeconds * (itemCount - 1));
                    attacker.AddTimedBuff(RoR2Content.Buffs.Cloak, stackTime);
                }
            }
        }
    }
}
