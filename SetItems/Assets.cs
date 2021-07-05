using Faust.SetItems.Auras;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.SetItems
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle;

        // Effects
        public static GameObject fireAuraCirclePrefab;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        public static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Faust.SetItems.itemsets_assets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(stream);
                }
            }

            LoadAuraEffect();
        }

        private static void LoadAuraEffect()
        {
            var aura = mainAssetBundle.LoadAsset<GameObject>("AuraCircle");
            aura.AddComponent<NetworkIdentity>();
            aura.AddComponent<NetworkedBodyAttachment>();
            aura.AddComponent<AuraController>();
            fireAuraCirclePrefab = aura.InstantiateClone("FireMagicCircle", true);
            Object.Destroy(aura);
        }
    }
}
