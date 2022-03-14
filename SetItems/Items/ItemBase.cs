using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Faust.SetItems.Items
{
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T Instance { get; private set; }

        public ItemBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBoilerplate/Item was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class ItemBase
    {
        public abstract string ItemName { get; }
        public abstract string ItemLangTokenName { get; }
        public abstract string ItemPickupDescription { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }

        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] ItemTags { get; set; } = { };

        public virtual bool CanRemove { get; } = true;
        public virtual bool Hidden { get; } = false;
        public virtual bool AIBlacklisted { get; set; } = false;

        public virtual GameObject ItemModel { get; } = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
        public virtual Sprite ItemIcon { get; } = LegacyResourcesAPI.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");

        //creates an unlockable requirement
        public virtual UnlockableDef Unlockable { get; }

        //creates necessary GameObject field for display rules
        public static GameObject ItemBodyModelPrefab;

        //initializes the item
        public virtual void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public abstract void CreateConfig(ConfigFile config);

        //actually creates instance of item
        public ItemDef ItemDef = ScriptableObject.CreateInstance<ItemDef>();

        //sets the lang tokens for in game use
        protected void CreateLang()
        {
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }

        //sets display rules
        public virtual ItemDisplayRuleDict CreateItemDisplayRules() {
            return null;
        }

        //actually defines the item
        protected void CreateItem()
        {
            if (AIBlacklisted)
            {
                ItemTags = new List<ItemTag>(ItemTags) { ItemTag.AIBlacklist }.ToArray();
            }

            ItemDef.name = "ITEM_" + ItemLangTokenName;
            ItemDef.nameToken = "ITEM_" + ItemLangTokenName + "_NAME";
            ItemDef.pickupToken = "ITEM_" + ItemLangTokenName + "_PICKUP";
            ItemDef.descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION";
            ItemDef.loreToken = "ITEM_" + ItemLangTokenName + "_LORE";
            ItemDef.pickupModelPrefab = ItemModel;
            ItemDef.pickupIconSprite = ItemIcon;
            ItemDef.hidden = Hidden;
            ItemDef.canRemove = CanRemove;
            ItemDef.tier = Tier;
            ItemDef.unlockableDef = Unlockable;
            
            if(ItemTags.Length > 0)
                ItemDef.tags = ItemTags;

            var rules = CreateItemDisplayRules();

            ItemAPI.Add(new CustomItem(ItemDef, rules));
        }

        //where hooks go
        public virtual void Hooks() { }

        //gets count of item from CharacterBody or CharacterMaster
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory)
            {
                return 0;
            }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory)
            {
                return 0;
            }

            return master.inventory.GetItemCount(ItemDef);
        }

        public bool HasItem(CharacterBody body)
        {
            return GetCount(body) > 0;
        }
    }
}