using Faust.SetItems.Items.FireGod;
using ItemStats;
using ItemStats.Stat;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Faust.SetItems.SoftDependencies
{
    public static class ItemStatsSoftDependency
    {
        public const string PluginGUID = "dev.ontrigger.itemstats";

        public static void AddItemStatDef(ItemDef itemDef, ItemStatDef itemStatDef, List<Type> statModifiers = null)
        {
            if (itemDef != null)
            {
                ItemStatsMod.AddCustomItemStatDef(itemDef.itemIndex, itemStatDef);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void CreateFireGodsWand1StatDef()
        {
            var cuthroatChestStafDef = new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat
                    (
                        (itemCount, ctx) => FireGodsWand1.BaseDurationOfBuffInSeconds + (FireGodsWand1.AdditionalDurationOfBuffInSeconds * (itemCount - 1)),
                        (value, ctx) => $"Current Max Buff Duration: {value} second(s)"
                    )
                }
            };
            AddItemStatDef(FireGodsWand1.Instance.ItemDef, cuthroatChestStafDef);
        }
    }
}

