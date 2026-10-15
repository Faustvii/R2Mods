# Quality of life chests

- Hides chests when they are empty.
- Hides used shop terminals.
- Highlights chests and interactables.

For feature suggestions or bug reports go [here](https://github.com/Faustvii/R2Mods/issues)

## QoLChests API for mod developers

QoLChests has a public API. Use it to add your interactables to the highlight and hide features.

### Getting started

Everything below here is just a suggestion and not the only way to do things, you do whatever you want.

Download QoLChests from Thunderstore. Add a reference to `QoLChests.dll` in your project:

```xml
<Reference Include="QoLChests">
  <HintPath>libs\QoLChests.dll</HintPath>
  <Private>false</Private>
</Reference>
```

Add the namespace:

```csharp
using Faust.QoLChests.Api;
```

Add QoLChests as a dependency of your plugin:

```csharp
[BepInDependency("Faust.QoLChests", BepInDependency.DependencyFlags.SoftDependency)]
```

Use a soft dependency so your mod works without QoLChests.

### Soft dependency

The game loads QoLChests types when it compiles a method that uses them. If QoLChests is not installed, that method fails, even if the call is inside an `if` that does not run.

Put all calls to `QoLChestsApi` in a separate wrapper class. Add `NoInlining` to each method, so the calls stay in the wrapper:

```csharp
using System.Runtime.CompilerServices;
using Faust.QoLChests.Api;
using UnityEngine;

internal static class QoLChestsIntegration
{
    public static bool Enabled =>
        BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Faust.QoLChests");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Register()
    {
        QoLChestsApi.RegisterInteractable("MyChest", InteractableCategory.Chest);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void MarkUsed(GameObject interactable) =>
        QoLChestsApi.MarkInteractableUsed(interactable);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void MarkAvailable(GameObject interactable) =>
        QoLChestsApi.MarkInteractableAvailable(interactable);
}
```

Call the wrapper only if QoLChests is installed:

```csharp
if (QoLChestsIntegration.Enabled)
    QoLChestsIntegration.Register();
```

Do not use QoLChests types outside the wrapper class unless you want a bad time.

### Register interactables

Call these in your plugin's `Awake`.

Register an interactable prefab. Use the prefab name without `(Clone)`:

```csharp
QoLChestsApi.RegisterInteractable("MyChest", InteractableCategory.Chest);
```

The category sets which highlight, color and hide settings apply.

If your prefabs are in an asset bundle, also register the bundle. The bundle must be loaded before the game finishes loading:

```csharp
QoLChestsApi.RegisterAssetBundle("myassetbundle");
```

To stop QoLChests from hiding/fading an interactable, blacklist it:

```csharp
QoLChestsApi.BlackListInteractable("MyTerminal");
```

A blacklisted interactable is still highlighted if it is registered. If you do not want it to be highlighted do not register it.

### Mark interactables as used

QoLChests uses the vanilla signals to know when an interactable is used/available. Most mods do not need this section.

If your mod blocks these signals, tell QoLChests when an interactable is used:

```csharp
QoLChestsApi.MarkInteractableUsed(gameObject);
```

If the interactable becomes usable again:

```csharp
QoLChestsApi.MarkInteractableAvailable(gameObject);
```

These calls are client side only. Call them on each client that must show the change.

With a soft dependency, call them through the wrapper from your hook:

```csharp
private static void OnChestOpenedLocally(GameObject chest)
{
    if (QoLChestsIntegration.Enabled)
        QoLChestsIntegration.MarkUsed(chest);
}
```
