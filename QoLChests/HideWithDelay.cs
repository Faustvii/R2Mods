using System.Collections;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a Renderer component to disable it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class HideWithDelay(float Delay = 1f) : MonoBehaviour
{
    private Collider[] colliders;
    private Renderer[] renderers;

    public void DisableRendererAfterDelay()
    {
        StartCoroutine(DisableRendererCoroutine());
    }

    public HideWithDelay SetDelay(float delay)
    {
        Delay = delay;
        return this;
    }

    private IEnumerator DisableRendererCoroutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(Delay);

        renderers = Utils.DisableRenderers(gameObject);
        colliders = Utils.DisableColliders(gameObject);
        Log.LogInfo(
            $"HideWithDelay DisableRenderers {renderers.Length} DisableColliders {colliders.Length}"
        );
        Utils.DisableHighlights(gameObject);

        Utils.DisableCommonVisualEffects(gameObject);
    }

    public void OnDestroy()
    {
        Log.LogInfo(
            $"HideWithDelay OnDestroy {gameObject.name} colliders {colliders.Length} renderers {renderers.Length}"
        );
        foreach (var collider in colliders)
        {
            if (collider)
                collider.enabled = true;
        }
        foreach (var renderer in renderers)
        {
            if (renderer)
                renderer.enabled = true;
        }
    }
}
