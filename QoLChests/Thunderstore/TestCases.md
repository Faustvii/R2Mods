# Test Cases

Below are some tests cases that should be run to confirm updates work as expected

## Chests

| Case                 | Expected Outcome                                |
| -------------------- | ----------------------------------------------- |
| Artifact of Delusion | Chests should become visible and be highlighted |
| Artifact of Delusion | Chests should still be interactable             |
| Used chest           | Should disappear and highlight should disable   |
| Used chest           | Should not have a collision                     |

## Shops

| Case              | Expected Outcome                                                |
| ----------------- | --------------------------------------------------------------- |
| Executive Card    | Shops can be used multiple times                                |
| Shipping manifest | Free shops should be highlighted                                |
| Shipping manifest | Used free shops should properly hide/fade (Including particles) |
| Shipping manifest | Used free shops have no collision                               |
| Used Shop         | Should disappear and highlight should disable                   |
| Stage change      | Should not remove item highlight in shops                       |
| Setting change    | Should not remove item highlight in shops                       |

## Settings

| Case              | Expected Outcome                           |
| ----------------- | ------------------------------------------ |
| Changing settings | Should take effect immediately             |
| Changing settings | Should not make used stuff reappear        |
| Fade              | Stuff will fade as expected                |
| Hide              | Stuff will hide                            |
| Different colors  | Categories can have different colors       |
| Used              | Highlights are removed if remove from used |
| Range highlighting | Disabled by default; current global highlighting remains unchanged |
| Range highlighting | Enabled interactables highlight inside the configured distance and disable outside it |
| Range highlighting | Changing the range or toggle takes effect immediately |
| Range highlighting | Used, hidden, or faded interactables are not incorrectly re-highlighted |

## Hexes and mimics

Run these cases as both host and client.

| Case | Expected outcome |
| --- | --- |
| Mimic spawns during a stage | The disguised mimic uses the chest color, strength, and distance rules |
| Chest highlights disabled | The disguised mimic has no QoL highlight |
| Mimic reveal and death | The game controls the visual lifetime without a custom removal hook |
| Used chests hidden or faded | Opening a mimic does not hide or fade the enemy |
| Configuration changes | Existing mimics use the new color, strength, category, and distance values |
| Blindness configuration disabled | Picking up Blindness leaves highlights enabled |
| Blindness configuration enabled | Picking up Blindness disables highlights while the player stands still |
| Blindness cleansed | Eligible highlights return while the player stands still |
| Blindness without range highlighting | Pickup and cleansing still update highlights |
| Blindness with range highlighting | Cleansing restores only highlights within range |
| Used, hidden, or faded interactable | Cleansing does not restore an ineligible highlight |
| Blindness stacks | Highlights stay disabled until the last active stack disappears |
| Teammate carries Blindness | The local player's highlights remain available |
| Respawn, stage transition, and no DLC | No stale suppression or exceptions occur |
| Multiple local players | A nearby local player without Blindness permits the shared highlight |

## General

These cover general cases, like restarting a run, going to the next stage

| Case               | Expected Outcome                                             |
| ------------------ | ------------------------------------------------------------ |
| Stage change       | All configured interactables are highlighted                 |
| Stage change       | No errors about null pointers in console                     |
| Restarting a run   | No errors about null pointers, interactables are highlighted |
| Starting a new run | Interactables are highlighted                                |
