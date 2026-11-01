using System.Collections.Generic;
using EntityStates.Mimic.Monster;
using Faust.QoLChests.Api;
using Faust.QoLChests.Components;
using Faust.QoLChests.Handlers;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Hooks;

public static class MimicHooks
{
    public static void Register()
    {
        On.EntityStates.Mimic.Monster.IsDisguisedState.OnEnter += OnDisguised;
    }

    private static void OnDisguised(
        On.EntityStates.Mimic.Monster.IsDisguisedState.orig_OnEnter orig,
        IsDisguisedState self
    )
    {
        orig(self);

        var childLocator = self.GetModelChildLocator();
        if (!childLocator || !childLocator.TryFindChild("HighlightChest", out var chest))
            return;

        // Tag the visual child, not the enemy's PurchaseInteraction/ChestBehavior root.
        // The game owns this child's disguise visibility and lifetime.
        var marker = chest.GetComponent<InteractableHighlightCategoryMarker>();
        if (!marker)
            marker = chest.gameObject.AddComponent<InteractableHighlightCategoryMarker>();
        marker.SetCategory(InteractableCategory.Mimic);

        var highlight = chest.GetComponent<Highlight>();
        if (!highlight)
            highlight = chest.gameObject.AddComponent<Highlight>();

        if (!highlight.highlightTargetValid)
        {
            var model = self.GetModelTransform();
            if (model)
            {
                var renderers = new List<Renderer>();
                foreach (var renderer in model.GetComponentsInChildren<Renderer>())
                    if (renderer is MeshRenderer or SkinnedMeshRenderer)
                        renderers.Add(renderer);
                highlight.SetTargetRendererList(renderers);
            }
        }

        InteractableStateHandler.TrackInteractable(chest.gameObject);
    }
}
