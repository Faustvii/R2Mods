using BepInEx.Configuration;
using Faust.SetItems.Auras;
using Faust.SetItems.Extensions;
using Faust.Shared.Compatability;
using R2API;
using RoR2;
using System.Collections.Generic;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsSet : ItemBaseSet<FireGodsSet>
    {
        public override List<ItemBase> SetItems => new List<ItemBase>
        {
            FireGodsHaste.Instance,
            FireGodsShield.Instance,
            FireGodsWings.Instance,
            FireGodsBurn.Instance,
        };

        public static ConfigEntry<float> AttackSpeedIncreaseInPercentage;
        public static ConfigEntry<float> AuraRadius;
        public static ConfigEntry<float> AuraDamageMultiplier;
        public static ConfigEntry<float> AdditionalDurationOfBuffInSeconds;
        public static ConfigEntry<float> ActiviationPercentChance;
        public static ConfigEntry<bool> ApplyTwoPieceBonusToEngineerTurrets;

        private static DotController.DotIndex[] BurnDots => new[]{
            DotController.DotIndex.Burn,
            DotController.DotIndex.StrongerBurn,
            DotController.DotIndex.PercentBurn,
        };

        public override string SetName => "Fire God";

        public override void CreateConfig(ConfigFile config)
        {
            AttackSpeedIncreaseInPercentage = config.Bind("Set: " + nameof(FireGodsSet), $"Attack speed increase with two piece bonus", 0.5f, $"How much attack speed increase should the two set bonus give.");
            ApplyTwoPieceBonusToEngineerTurrets = config.Bind("Set: " + nameof(FireGodsSet), $"Should the two piece bonus apply to Engineer turrets?", true, $"If Engineer turrets will get the two piece set bonus.");
            AuraRadius = config.Bind("Set: " + nameof(FireGodsSet), $"Aura radius?", 9f, $"How big of a radius does the 4 piece set bonus cover.");
            AuraDamageMultiplier = config.Bind("Set: " + nameof(FireGodsSet), $"Aura damage multiplier?", 4f, $"How much should the aura multiply the characters base damage with.");
        }

        public override void Hooks()
        {
            base.Hooks();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            RoR2Application.onLoad += OnLoadModCompat;
        }

        private void OnLoadModCompat()
        {
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.AddSliderToPercentageOptionsDecimal(AttackSpeedIncreaseInPercentage);
                RiskOfOptionsCompat.AddSliderNumberOptions(AuraRadius, AuraDamageMultiplier);
                RiskOfOptionsCompat.AddCheckboxOptions(ApplyTwoPieceBonusToEngineerTurrets);
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Buffs.Buffs.FireGodsWrath))
            {
                args.attackSpeedMultAdd += AttackSpeedIncreaseInPercentage.Value;
            }
        }

        public override void ApplyTwoPieceEffect(CharacterBody body, bool isPlayer)
        {
            if (!isPlayer && !ApplyTwoPieceBonusToEngineerTurrets.Value)
                return;

            base.ApplyTwoPieceEffect(body, isPlayer);
            body.AddBuff(Buffs.Buffs.FireGodsWrath);
            if (isPlayer)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel power surge through you as you don two of the four wands" });
            }
        }

        public override void ApplyFourPieceEffect(CharacterBody body, bool isPlayer)
        {
            base.ApplyFourPieceEffect(body, isPlayer);
            body.AddItemBehavior<FireGodItemBehavior>(1);
            if (isPlayer)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel power surge through you as you don all four wands" });
            }
        }

        public override void RemoveTwoPieceEffect(CharacterBody body, bool isPlayer)
        {
            base.RemoveTwoPieceEffect(body, isPlayer);
            body.RemoveBuff(Buffs.Buffs.FireGodsWrath);
            if (isPlayer)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel weakened as you only have one of the four wands" });
            }
        }

        public override void RemoveFourPieceEffect(CharacterBody body, bool isPlayer)
        {
            base.RemoveFourPieceEffect(body, isPlayer);
            body.RemoveItemBehavior<FireGodItemBehavior>();
            if (isPlayer)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel weakened as you loose the power of the four wands" });
            }
        }
    }
}
