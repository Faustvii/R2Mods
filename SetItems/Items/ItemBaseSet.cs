using BepInEx.Configuration;
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
        public virtual List<CharacterBody> PlayersWithTwoSetBuff { get; } = new List<CharacterBody>();
        public virtual List<CharacterBody> PlayersWithFourSetBuff { get; } = new List<CharacterBody>();
        public abstract string SetName { get; }

        public virtual void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (!self || !self.isPlayerControlled || !NetworkServer.active)
            {
                return;
            }

            var equippedPieces = UniqueEquippedSetItems(self);
            if (equippedPieces >= 2)
            {
                if (!PlayersWithTwoSetBuff.Any(x => x == self))
                    ApplyTwoPieceEffect(self);
            }
            else
            {
                if (PlayersWithTwoSetBuff.Any(x => x == self))
                    RemoveTwoPieceEffect(self); ;
            }
            if (equippedPieces >= 4)
            {
                if (!PlayersWithFourSetBuff.Any(x => x == self))
                    ApplyFourPieceEffect(self);
            }
            else
            {
                if (PlayersWithFourSetBuff.Any(x => x == self))
                    RemoveFourPieceEffect(self);
            }

        }

        public virtual void Init(ConfigFile config)
        {
            Hooks();
        }

        public int UniqueEquippedSetItems(CharacterBody body)
        {
            return SetItems.Count(x => x.HasItem(body));
        }

        public virtual void ApplyTwoPieceEffect(CharacterBody body)
        {
            PlayersWithTwoSetBuff.Add(body);
        }

        public virtual void ApplyFourPieceEffect(CharacterBody body)
        {
            PlayersWithFourSetBuff.Add(body);
        }

        public virtual void RemoveTwoPieceEffect(CharacterBody body)
        {
            PlayersWithTwoSetBuff.Remove(body);
        }

        public virtual void RemoveFourPieceEffect(CharacterBody body)
        {
            PlayersWithFourSetBuff.Remove(body);
        }
    }
}
