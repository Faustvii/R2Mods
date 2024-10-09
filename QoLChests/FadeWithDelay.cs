using System.Collections;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a interactable component to "fade" it after a specified delay.
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

        Utils.DisableRenderers(gameObject);
        Utils.DisableColliders(gameObject);
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
        Destroy(this);
    }
}
