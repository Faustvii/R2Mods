using Faust.SetItems.Items.FireGod;
using Faust.SetItems.Utils;
using Faust.Shared;
using Faust.Shared.Compatability;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Faust.SetItems.Buffs
{
    static class Buffs
    {
        public static BuffDef FireGodsWrath;
        //public static BuffDef Icon1;
        //public static BuffDef Icon2;
        //public static BuffDef Icon3;
        //public static BuffDef Icon4;
        //public static BuffDef Icon5;

        internal static List<BuffDef> BuffDefs = new List<BuffDef>();

        public static void RegisterBuffs()
        {
            FireGodsWrath = AddNewBuff("Fire God's Wrath", CreateSpriteFromPathOrDefault("RoR2/Base/Common/MiscIcons/texAttackIcon.png"), Color.red, false, false);
            //Icon1 = AddNewBuff("1", CreateSpriteFromPathOrDefault("RoR2/Base/Common/MiscIcons/texAttackIcon.png"), Color.red, false, false);
            //Icon2 = AddNewBuff("2", CreateSpriteFromPathOrDefault("RoR2/Base/Common/MiscIcons/texMysteryIcon.png"), Color.red, false, false);
            //Icon3 = AddNewBuff("3", CreateSpriteFromPathOrDefault("RoR2/Base/PersonalShield/texPersonalShieldIcon.png"), Color.red, false, false);
            //Icon4 = AddNewBuff("4", CreateSpriteFromPathOrDefault("RoR2/Base/Common/ColorRamps/texRampGenericFire.png"), Color.red, false, false);
            //Icon5 = AddNewBuff("5", CreateSpriteFromPathOrDefault("RoR2/Base/Common/MiscIcons/texWIPIcon.png"), Color.red, false, false);

            //FireGodsShield = AddNewBuff("Fire God's Shield", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.red, false, false);
            //RoR2Content.Buffs.HiddenInvincibility.iconSprite.texture.
            OnLoadModCompat();
        }

        internal static Sprite CreateSpriteFromPathOrDefault(string path)
        {
            var icon = Addressables.LoadAssetAsync<Texture2D>(path).WaitForCompletion();
            Log.LogDebug($"Icon '{path}' Not null: {icon != null} - {icon?.name} - {icon?.GetType()}");
            var sprite = CreateSpriteFromTexture(icon);

            return sprite ?? LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texMysteryIcon");
        }

        internal static Sprite CreateSpriteFromTexture(UnityEngine.Texture2D texture)
        {
            if (texture == null)
                return null;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static void OnLoadModCompat()
        {
            if (BetterUICompat.IsInstalled)
            {
                var voidInstabilityDebuffInfo = BetterUICompat.CreateBetterUIBuffInformation($"{nameof(FireGodsWrath)}", FireGodsWrath.name, $"Increases your attack speed by {MathHelpers.FloatToPercentageString(FireGodsSet.AttackSpeedIncreaseInPercentage.Value)}");
                BetterUICompat.RegisterBuffInfo(FireGodsWrath, voidInstabilityDebuffInfo.nameToken, voidInstabilityDebuffInfo.descToken);
            }
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            BuffDefs.Add(buffDef);

            return buffDef;
        }
    }
}
