using System.Collections;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a Renderer component to disable it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class FadeWithDelay(float Delay = 1f) : MonoBehaviour
{
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
        var baseRenderer = GetComponent<Renderer>();
        var childRenderer = GetComponentInChildren<Renderer>();
        Renderer[] allRenderers = [baseRenderer, childRenderer];
        foreach (var renderer in allRenderers)
        {
            if (renderer)
            {
                renderer.enabled = false;
            }
        }
        var highlight = GetComponent<Highlight>();
        if (highlight)
        {
            highlight.isOn = true;
            highlight.enabled = true;
            highlight.CustomColor = new Color32(128, 128, 128, 64);
            highlight.highlightColor = Highlight.HighlightColor.custom;
        }
        Destroy(this);
    }
}
