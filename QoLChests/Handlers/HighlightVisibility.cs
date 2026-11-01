using Faust.QoLChests.Configs;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Handlers;

/// <summary>
/// Shared player limits for both periodic updates and direct highlight restores.
/// </summary>
public static class HighlightVisibility
{
    public static bool RequiresUpdates =>
        ModConfig.Instance.HighlightOnlyInRange.Value
        || ModConfig.Instance.DisableHighlightsDuringBlindness.Value;

    public static bool Allows(Vector3 position)
    {
        var config = ModConfig.Instance;
        var rangeMode = config.HighlightOnlyInRange.Value;
        var respectBlindness = config.DisableHighlightsDuringBlindness.Value;
        if (!rangeMode && !respectBlindness)
            return true;

        var distance = config.HighlightDistance.Value;
        var distanceSquared = distance * distance;
        var hasLocalBody = false;

        // Highlights are shared by local cameras, as with range highlighting.
        foreach (var localUser in LocalUserManager.readOnlyLocalUsersList)
        {
            var body = localUser?.cachedBody;
            if (!body || !body.isPlayerControlled)
                continue;

            hasLocalBody = true;
            if (respectBlindness && body.inventory && DLC4Content.Items.Blindness
                && body.inventory.GetItemCountEffective(DLC4Content.Items.Blindness) > 0)
                continue;

            if (!rangeMode || (body.corePosition - position).sqrMagnitude <= distanceSquared)
                return true;
        }

        return !rangeMode && !hasLocalBody;
    }
}
