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

    public static void DisableRenderers(GameObject gameObject)
    {
        var allRenderers = GetRenderers(gameObject);
        foreach (var renderer in allRenderers)
        {
            if (renderer)
            {
                renderer.enabled = false;
            }
        }
    }

    public static void DisableColliders(GameObject gameObject)
    {
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (var collider in allColliders)
        {
            if (collider)
            {
                collider.enabled = false;
            }
        }
    }

    public static Highlight[] GetHighlights(GameObject gameObject)
    {
        var baseHighlight = gameObject.GetComponent<Highlight>();
        var childHighlights = gameObject.GetComponentsInChildren<Highlight>();
        Highlight[] allHighlights = [baseHighlight, .. childHighlights];

        return allHighlights;
    }

    public static void DisableHighlights(GameObject gameObject)
    {
        var allHighlights = GetHighlights(gameObject);
        foreach (var highlight in allHighlights)
        {
            if (highlight)
            {
                highlight.isOn = false;
                highlight.enabled = false;
            }
        }
    }

    public static void DisableCommonVisualEffects(GameObject gameObject)
    {
        DisableParticleSystem(gameObject);
        DisableTrailRenderer(gameObject);
        DisableLights(gameObject);
    }

    private static void DisableParticleSystem(GameObject gameObject)
    {
        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(); // Stop the particle system
            ps.gameObject.SetActive(false); // Or disable it completely
        }
    }

    private static void DisableTrailRenderer(GameObject gameObject)
    {
        TrailRenderer[] trailRenderers = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.enabled = false; // Disable trail renderer
        }
    }

    private static void DisableLights(GameObject gameObject)
    {
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = false; // Disable the light component
        }
    }
}
