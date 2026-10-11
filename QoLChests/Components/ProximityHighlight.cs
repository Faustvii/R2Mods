using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Components;

/// <summary>
/// Applies range-based highlighting to a live interactable without scanning the scene.
/// </summary>
public class ProximityHighlight : MonoBehaviour
{
    private const float UpdateInterval = 0.1f;
    private float nextCheckTime;
    private bool isInRange;
    private bool hasCheckedRange;

    public void Configure(bool rangeMode)
    {
        this.enabled = rangeMode;
        hasCheckedRange = false;
    }

    private void Update()
    {
        if (Time.unscaledTime < nextCheckTime)
            return;

        nextCheckTime = Time.unscaledTime + UpdateInterval;
        var distance = ModConfig.Instance.HighlightDistance.Value;
        var distanceSquared = distance * distance;
        var closestPlayerDistanceSquared = float.MaxValue;

        foreach (var localUser in LocalUserManager.readOnlyLocalUsersList)
        {
            var body = localUser?.cachedBody;
            if (!body || !body.isPlayerControlled)
                continue;

            var playerDistanceSquared = (body.corePosition - transform.position).sqrMagnitude;
            if (playerDistanceSquared < closestPlayerDistanceSquared)
                closestPlayerDistanceSquared = playerDistanceSquared;
        }

        var nowInRange = closestPlayerDistanceSquared <= distanceSquared;
        if (hasCheckedRange && nowInRange == isInRange)
            return;

        hasCheckedRange = true;
        isInRange = nowInRange;

        if (!nowInRange)
        {
            HighlightHandler.Disable(gameObject);
            return;
        }

        if (GetComponent<HideWithDelay>() || GetComponent<FadeWithDelay>())
            return;

        HighlightHandler.Enable(gameObject);
    }
}
