using Faust.QoLChests.Configs;
using UnityEngine;

namespace Faust.QoLChests;

public static class Constants
{
    public static HighlightableResource[] ChestResourcesPaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Barrel1/Barrel1.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/CasinoChest/CasinoChest.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/CategoryChest/CategoryChestDamage.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/CategoryChest/CategoryChestHealing.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/CategoryChest/CategoryChestUtility.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Chest1/Chest1.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Chest2/Chest2.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/EquipmentBarrel/EquipmentBarrel.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/GoldChest/GoldChest.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/LunarChest/LunarChest.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Damage Variant.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Healing Variant.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Utility Variant.prefab",
            InteractableCategory.Chest
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/DLC1/VoidChest/VoidChest.prefab",
            InteractableCategory.Chest
        ),
    ];

    public static HighlightableResource[] StealthedChestResourcePaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Chest1StealthedVariant/Chest1StealthedVariant.prefab",
            InteractableCategory.Chest
        ),
    ];

    public static HighlightableResource[] ShopResourcePaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/LunarShopTerminal/LunarShopTerminal.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/MultiShopEquipmentTerminal/MultiShopEquipmentTerminal.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/MultiShopLargeTerminal/MultiShopLargeTerminal.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/MultiShopTerminal/MultiShopTerminal.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/MultiShopTerminal/ShopTerminal.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/TripleShop/TripleShop.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/TripleShopLarge/TripleShopLarge.prefab",
            InteractableCategory.Shop
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/TripleShopEquipment/TripleShopEquipment.prefab",
            InteractableCategory.Shop
        )
    ];
    public static HighlightableResource[] ScrapperResourcePaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Scrapper/Scrapper.prefab",
            InteractableCategory.Scrapper
        ),
    ];

    public static HighlightableResource[] DuplicatorResourcesPaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Duplicator/Duplicator.prefab",
            InteractableCategory.Duplicator
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/DuplicatorLarge/DuplicatorLarge.prefab",
            InteractableCategory.Duplicator
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/DuplicatorMilitary/DuplicatorMilitary.prefab",
            InteractableCategory.Duplicator
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/DuplicatorWild/DuplicatorWild.prefab",
            InteractableCategory.Duplicator
        ),
    ];

    public static HighlightableResource[] DroneResourcesPaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/Drone1Broken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/Drone2Broken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/EmergencyDroneBroken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/EquipmentDroneBroken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/FlameDroneBroken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/MegaDroneBroken.prefab",
            InteractableCategory.Drone
        ),
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/MissileDroneBroken.prefab",
            InteractableCategory.Drone
        ),
    ];

    public static HighlightableResource[] TurrentResourcePaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/Base/Drones/Turret1Broken.prefab",
            InteractableCategory.Turret
        ),
    ];

    public static HighlightableResource[] ArtifactOfDevotionResourcePaths =
    [
        HighlightableResource.CreateFromResourcePath(
            "RoR2/CU8/LemurianEgg/LemurianEgg.prefab",
            InteractableCategory.Drone
        )
    ];

    public static HighlightableResource[] Starstorm2Interactables =
    [
        new("ShockDroneBroken", "", InteractableCategory.Drone),
        new("CloneDroneBroken", "", InteractableCategory.Drone),
        new("DroneTablePrefab", "", InteractableCategory.Duplicator),
    ];

    public static Color32 GetColor(ConfigHighlightColor color) =>
        color switch
        {
            ConfigHighlightColor.Yellow => Yellow,
            ConfigHighlightColor.Red => Red,
            ConfigHighlightColor.Blue => Blue,
            ConfigHighlightColor.Pink => Pink,
            ConfigHighlightColor.Orange => Orange,
            ConfigHighlightColor.Green => Green,
            ConfigHighlightColor.Cyan => Cyan,
            ConfigHighlightColor.Violet => Violet,
            ConfigHighlightColor.Magenta => Magenta,
            ConfigHighlightColor.Gray => Gray,
            ConfigHighlightColor.White => White,
            _ => Yellow
        };

    private static Color32 Yellow => new(255, 255, 1, 255);
    private static Color32 Red => new(255, 1, 1, 255);
    private static Color32 Blue => new(1, 1, 255, 255);
    private static Color32 Pink => new(255, 1, 127, 255);
    private static Color32 Orange => new(255, 128, 1, 255);
    private static Color32 Green => new(1, 255, 1, 255);
    private static Color32 Cyan => new(1, 255, 255, 255);
    private static Color32 Violet => new(127, 1, 255, 255);
    private static Color32 Magenta => new(255, 1, 255, 255);
    private static Color32 Gray => new(128, 128, 128, 255);
    private static Color32 White => new(255, 255, 255, 255);
}


// Add lockbox
// free chest
// void triple?
// lockbox void
// void supressor
// void coin barrel
