using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using UnityEngine;

namespace Faust.SetItems
{
    internal class CustomContentPack : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => Main.PluginGUID;
        internal ItemDef[] Items;
        internal EffectDef[] Effects;
        internal GameObject[] NetworkPrefabs;
        internal BuffDef[] Buffs;

        internal void Init(ItemDef[] itemDefs, EffectDef[] effects, GameObject[] networkPrefabs, BuffDef[] buffs)
        {
            Items = itemDefs;
            Effects = effects;
            NetworkPrefabs = networkPrefabs;
            Buffs = buffs;
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            contentPack.identifier = identifier;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
            //contentPack.itemDefs.Add(Items);
            contentPack.networkedObjectPrefabs.Add(NetworkPrefabs);
            contentPack.buffDefs.Add(Buffs);
            args.ReportProgress(1f);
            yield break;
        }
    }
}
