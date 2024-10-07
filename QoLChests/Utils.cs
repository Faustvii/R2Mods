using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

public static class Utils
{
    public static Renderer[] GetRenderers(GameObject gameObject)
    {
        var modelLocator = gameObject.GetComponent<ModelLocator>();
        var childRenderers =
            modelLocator?.modelTransform?.GetComponentsInChildren<Renderer>()
            ?? gameObject.GetComponentsInChildren<Renderer>();
        var baseRenderers =
            modelLocator?.modelTransform?.GetComponents<Renderer>()
            ?? gameObject.GetComponents<Renderer>();
        Renderer[] allRenderers = [.. baseRenderers, .. childRenderers];

        return allRenderers;
    }
}
