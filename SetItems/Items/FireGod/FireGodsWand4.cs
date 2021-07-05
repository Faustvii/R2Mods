using BepInEx.Configuration;
using Faust.SetItems.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsWand4 : ItemBase<FireGodsWand4>
    {
        public static ConfigOption<float> BaseDurationOfBuffInSeconds;
        public static ConfigOption<float> AdditionalDurationOfBuffInSeconds;
        public static ConfigOption<float> ActiviationPercentChance;

        public override string ItemLangTokenName => nameof(FireGodsWand4);
        public override string ItemName => "Fire Gods Wand 4";
        public override string ItemPickupDescription => "Chance to cloak on kill";
        public override string ItemFullDescription => $"Whenever you <style=cIsDamage>kill an enemy</style>, you have a <style=cIsUtility>{MathHelpers.FloatToPercentageString(ActiviationPercentChance)}</style> chance to cloak for <style=cIsUtility>{BaseDurationOfBuffInSeconds}s</style> <style=cStack>(+{AdditionalDurationOfBuffInSeconds}s per stack)</style>.";
        public override string ItemLore => "Those who visit in the night are either praying for a favour, or preying on a neighbour.";

        public override ItemTier Tier => ItemTier.Tier2;

        public override Sprite ItemIcon => Assets.mainAssetBundle?.LoadAsset<Sprite>("FireGodsWand4Icon.png") ?? base.ItemIcon;
        public override GameObject ItemModel => Assets.mainAssetBundle?.LoadAsset<GameObject>("FireGodsWand4.prefab") ?? base.ItemModel;

        public override void CreateConfig(ConfigFile config)
        {
            BaseDurationOfBuffInSeconds = config.ActiveBind("Item: " + ItemLangTokenName, $"Base Duration of Buff with One {ItemLangTokenName}", 4f, $"How many seconds should {ItemName} buff last with a single {ItemName}?");
            ActiviationPercentChance = config.ActiveBind("Item: " + ItemLangTokenName, "Activation Percent Chance", 0.2f, "What chance in percentage should the cloaking ability trigger?");
            AdditionalDurationOfBuffInSeconds = config.ActiveBind("Item: " + ItemLangTokenName, $"Additional Duration of Buff per {ItemLangTokenName} Stack", 0.5f, $"How many additional seconds of buff should each {ItemName} after the first one give?");
        }

        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
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
