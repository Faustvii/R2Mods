using System.Collections;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a Renderer component to disable it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class HideWithDelay(float Delay = 1f) : MonoBehaviour
{
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

        Utils.DisableColliders(gameObject);
        Utils.DisableRenderers(gameObject);
        Utils.DisableHighlights(gameObject);

        Utils.DisableCommonVisualEffects(gameObject);

        Destroy(this);
    }
}
