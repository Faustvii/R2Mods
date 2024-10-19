using System.Collections;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a interactable component to "fade" it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class FadeWithDelay(float Delay = 1f) : MonoBehaviour
{
    private Collider[] colliders;
    private Renderer[] renderers;

    public void DisableRendererAfterDelay()
    {
        StartCoroutine(DisableRendererCoroutine());
    }

    public FadeWithDelay SetDelay(float delay)
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
        Log.LogInfo($"FadeWithDelay DisableRenderers {renderers.Length} DisableColliders {colliders.Length}");
        Utils.DisableCommonVisualEffects(gameObject);
        // For some reason when the renderer is disabled but it has a highlight on it will look faded instead of hidden.
        var highlight = GetComponent<Highlight>();
        if (highlight)
        {
            highlight.isOn = true;
            highlight.enabled = true;
            highlight.CustomColor = new Color32(128, 128, 128, 64);
            highlight.highlightColor = Highlight.HighlightColor.custom;
        }
    }

    public void OnDestroy()
    {
        Log.LogInfo($"FadeWithDelay OnDestroy {gameObject.name} colliders {colliders.Length} renderers {renderers.Length}");
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}
