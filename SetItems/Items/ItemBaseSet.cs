using BepInEx.Configuration;
using Faust.Shared;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Faust.SetItems.Items
{
    public abstract class ItemBaseSet<T> : ItemBaseSet where T : ItemBaseSet<T>
    {
        public static T Instance { get; private set; }

        public ItemBaseSet()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBoilerplate/Item was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class ItemBaseSet
    {
        public abstract List<ItemBase> SetItems { get; }
        public virtual List<CharacterBody> EntitiesWithTwoSetBuff { get; } = new List<CharacterBody>();
        public virtual List<CharacterBody> EntitiesWithFourSetBuff { get; } = new List<CharacterBody>();
        public abstract string SetName { get; }
        public abstract void CreateConfig(ConfigFile config);

        public virtual void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.Inventory.CopyItemsFrom_Inventory_Func2 += Inventory_CopyItemsFrom_Inventory_Func2;
        }

        private void Inventory_CopyItemsFrom_Inventory_Func2(On.RoR2.Inventory.orig_CopyItemsFrom_Inventory_Func2 orig, Inventory self, Inventory other, Func<ItemIndex, bool> filter)
        {
            orig(self, other, filter);
            if (!NetworkServer.active)
                return;

            var turrentMaster = self.GetComponent<CharacterMaster>();
            var playerMaster = other.GetComponent<CharacterMaster>();
            if (!turrentMaster || !playerMaster)
                return;

            var playerBody = playerMaster.GetBody();
            var turretBody = turrentMaster.GetBody();
            if (!playerBody || !turretBody || !playerBody.isPlayerControlled)
            {
                Log.LogDebug($"is playerBody controlled by player: {playerBody?.isPlayerControlled} - {playerBody?.name}");
                return;
            }

            Log.LogDebug($"CopyTo: '{self.name}' - CopyFrom: '{other.name}'");
            var equippedPieces = UniqueEquippedSetItems(playerBody);
            if (equippedPieces >= 2)
            {
                ApplyTwoPieceEffect(turretBody, false);
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            if (!NetworkServer.active || !self || !self.isPlayerControlled)
            {
                return;
            }

            var equippedPieces = UniqueEquippedSetItems(self);
            if (equippedPieces >= 2)
            {
                if (!EntitiesWithTwoSetBuff.Any(x => x == self))
                    ApplyTwoPieceEffect(self, true);
            }
            else
            {
                if (EntitiesWithTwoSetBuff.Any(x => x == self))
                    RemoveTwoPieceEffect(self, true);
            }
            if (equippedPieces >= 4)
            {
                if (!EntitiesWithFourSetBuff.Any(x => x == self))
                    ApplyFourPieceEffect(self, true);
            }
            else
            {
                if (EntitiesWithFourSetBuff.Any(x => x == self))
                    RemoveFourPieceEffect(self, true);
            }

        }

        public virtual void Init(ConfigFile config)
        {
            CreateConfig(config);
            Hooks();
        }

        public int UniqueEquippedSetItems(CharacterBody body)
        {
            return SetItems.Count(x => x.HasItem(body));
        }

        public virtual void ApplyTwoPieceEffect(CharacterBody body, bool isPlayer)
        {
            EntitiesWithTwoSetBuff.Add(body);
        }

        public virtual void ApplyFourPieceEffect(CharacterBody body, bool isPlayer)
        {
            EntitiesWithFourSetBuff.Add(body);
        }

        public virtual void RemoveTwoPieceEffect(CharacterBody body, bool isPlayer)
        {
            EntitiesWithTwoSetBuff.Remove(body);
        }

        public virtual void RemoveFourPieceEffect(CharacterBody body, bool isPlayer)
        {
            EntitiesWithFourSetBuff.Remove(body);
        }
    }
}
