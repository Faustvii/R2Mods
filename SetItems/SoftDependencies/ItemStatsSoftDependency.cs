using Faust.SetItems.Items.FireGod;
using Faust.SetItems.Utils;
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
            AddItemStatDef(FireGodsHaste.Instance.ItemDef, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat
                    (
                        (itemCount, ctx) => FireGodsHaste.BaseAttackSpeedPercentageIncrease.Value + (FireGodsHaste.AdditionalAttackSpeedPercentageIncrease.Value * (itemCount - 1)),
                        (value, ctx) => $"Current Max Buff Duration: {value} second(s)"
                    )
                }
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void CreateFireGodsWand4StatDef()
        {
            AddItemStatDef(FireGodsBurn.Instance.ItemDef, new ItemStatDef
            {
                Stats = new List<ItemStat>()
                {
                    new ItemStat
                    (
                        (itemCount, ctx) => FireGodsBurn.ActiviationPercentChance.Value + (FireGodsBurn.AdditionalActiviationPercentChance.Value * (itemCount - 1)),
                        (value, ctx) => $"Current activation chance: {MathHelpers.FloatToPercentageString(value)}"
                    )
                }
            });
        }
    }
}

