using Faust.QoLChests.Handlers;
using UnityEngine;

namespace Faust.QoLChests.Components;

/// <summary>
/// Applies player visibility limits to a live interactable without scanning the scene.
/// </summary>
public class ProximityHighlight : MonoBehaviour
{
    private const float UpdateInterval = 0.1f;
    private float nextCheckTime;
    private bool isVisible;
    private bool hasCheckedVisibility;

    public void Configure(bool requiresUpdates)
    {
        enabled = requiresUpdates;
        hasCheckedVisibility = false;
        nextCheckTime = 0f;
    }

    private void Update()
    {
        if (Time.unscaledTime < nextCheckTime)
            return;

        nextCheckTime = Time.unscaledTime + UpdateInterval;
        var nowVisible = HighlightVisibility.Allows(transform.position);
        if (hasCheckedVisibility && nowVisible == isVisible)
            return;

        hasCheckedVisibility = true;
        isVisible = nowVisible;

        if (!nowVisible)
        {
            HighlightHandler.Disable(gameObject);
            return;
        }

        if (GetComponent<HideWithDelay>() || GetComponent<FadeWithDelay>())
            return;

        HighlightHandler.Enable(gameObject);
    }
}
