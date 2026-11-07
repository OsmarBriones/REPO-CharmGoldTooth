# CharmGoldTooth Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, cooldown display, host settings, debug mode).

## The power: Golden Bite

while the Gold Tooth is pocketed, pressing E (`InputKey.Interact`) while holding a food valuable consumes it and heals the owner 1.5 HP per $100 of its current value. Twin of the Tooth's Hearty Bite with the Gold Tooth numbers (the Tooth repository held only its stub when this was written, so the structure was designed independently; naming may differ).

- `GoldToothCharm : Charm` registers the prefab name `Valuable Museum GoldTooth`, wear 30% per bite, no cooldown, the settings below, the food list, the short-lived `RecentKeys` set and the host handler `OnHostRequest`.
- `GoldenBite` (a `MonoBehaviour` on the plugin object) runs on the owner's machine only while `Charm.IsActive`.
- `Domain/` (game-free, unit tested): `BiteHealing` (heal amount, whether to eat), `FoodList` (allowlist parsing and matching), `RecentKeys` (double-heal guard), `ValuableKey` (key rule).

### Flow of a bite
Owner machine (`GoldenBite.Update`):
1. `SemiFunc.InputDown(InputKey.Interact)` this frame.
2. `PhysGrabber.instance.grabbed` and `grabbedPhysGrabObject` give the held object; it needs a `ValuableObject` whose name is on the food list.
3. The local player must be alive; the key (PhotonView id in multiplayer, instance id alone) must not be in `RecentKeys`.
4. `BiteHealing.HealAmount(dollarValueCurrent, HealPerHundredDollars)`; `ShouldEat` refuses a heal of 0 and refuses full health unless `EatAtFullHealth` is on.
5. `Charm.TryUse()` wears the Gold Tooth (30%), then `PlayerHealth.Heal(amount, true)` heals locally (owner only, the game syncs the new health), then `Charm.SendToHost("eat", key)`.

Host (`GoldToothCharm.OnHostRequest`, reached through CharmCore's event 183, which already checked the sender carries the Gold Tooth): resolves the valuable from the key, checks it is on the host's food list, is held by the sender (`physGrabObject.playerGrabbing` contains a grabber whose `playerAvatar` is the sender) and is not pocketed (`CharmsApi.CarrierOf`), then calls `PhysGrabObject.DestroyPhysGrabObject()` (acts only on the master).

### Settings
| Key | Type | Range | Default | Meaning |
|---|---|---|---|---|
| `HealPerHundredDollars` | float, HP per $100 | 0 to 25 | 1.5 | 0 disables the heal (and so the bite). Not negative: eating must not hurt. 25 caps the heal. |
| `EatAtFullHealth` | float used as 0/1 | 0 to 1 | 0 | 0.5 and above is on. |
| `WearPercent` (CharmCore) | float | 0 to 100 | 30 | Wear per bite. |
| `FoodPrefabNames` | string (plain `Bind`, read once at startup, local) | n/a | the eight prefab names | Comma, semicolon or line separated; case-insensitive; `(Clone)` ignored; trailing `*` = prefix match. |

Default food prefabs (from the exported assets): `Valuable Museum Toast`, `Valuable Museum Banana Bow`, `Valuable Wizard Red Mushroom`, `Valuable Museum Gumball`, `Valuable Museum Egg`, `Valuable Museum Gem Burger`, `Valuable Museum Milk`, `Valuable Arctic Pills` (the only Pills prefab in the assets, so "all sizes" is covered).

## Game members used

Checked by `CharmGoldTooth.Tests/GameContractTests.cs`:
- `InputKey.Interact`, `SemiFunc.InputDown(InputKey)`, `SemiFunc.IsMultiplayer()`.
- `PhysGrabber.instance`, `.grabbed`, `.grabbedPhysGrabObject`, `.playerAvatar`.
- `PhysGrabObject.playerGrabbing`, `.photonView`, `.DestroyPhysGrabObject()`.
- `ValuableObject.dollarValueCurrent`.
- `PlayerHealth.Heal(int, bool)`, `.health`, `.maxHealth`; `PlayerAvatar.instance`, `.playerHealth`, `.deadSet`, `.isDisabled`.

No Harmony patch of its own.

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output.

## Known limits

- Looked up by exact prefab name; a game update that renames the prefab needs a mod update. The same holds for the food names.
- The food list is a local string. A client whose list is wider than the host's heals and wears the Gold Tooth, but the host refuses to consume the food; the player could repeat it. Self-inflicted wear only. Keep the lists equal.
- The Gold Tooth wears when the owner presses E, before the host confirms. If the host refuses (lag, food gone), the tooth wore for nothing; there is no confirmation channel.

## Not verified (needs the game)
The build and unit tests prove the logic and that the game members exist, not the behaviour in a running game. Unverified:
- That E (`Interact`) is not consumed by another system while holding a valuable, and that `grabbedPhysGrabObject` is set for held valuables.
- That `DestroyPhysGrabObject` on a held valuable leaves the grabber and haul list in a clean state on the host and clients.
- That `PlayerHealth.Heal` shows the effect and syncs health to other players in multiplayer.
- That `PhotonView.Find(key)` resolves the valuable on the host and that an `int` argument survives the Photon event.
- Whether `dollarValueCurrent` is in the unit the player sees as "$100" (assumed dollars).

## Decisions
- **Rounding:** nearest whole point, halves up, minimum 1 for any positive value and rate ($100 heals 2, $300 heals 5). A heal of 0 (rate 0) refuses the bite.
- **Full health:** refused by default (food kept, no wear); `EatAtFullHealth = 1` allows wasting it.
- **Local heal, host consumption:** health belongs to the owner's machine; destruction is master-only, so the owner sends `eat` and the host validates.
- **Double-heal guard:** `RecentKeys` remembers the eaten valuable for 3 s and is cleared when the charm deactivates. Key: view id in multiplayer, instance id alone (same rule as Pockets' `PocketedValuable.Key`).
- **No cooldown:** the food being consumed is the limit; `TryUse()` only applies the wear.
- **Dead players** cannot eat. The food list is not host-synced (CharmCore syncs numbers only).

## Runtime hygiene

Checked against `RepoKit/references/runtime-hygiene.md`, item by item (not verified in game; build and unit tests only).

1. Lifetime of references: fixed. `Resolve` no longer calls `Object.FindObjectsByType<PhysGrabObject>` (scene-wide). Alone it compares the key with the sender's held valuable, in multiplayer it uses `PhotonView.Find`; the validation rules (food list, held by sender, not pocketed) are unchanged. `Recent` (ids and times only) is now also cleared in `OnLevelEnded`, not only `OnDeactivated`.
2. Work frequency: `Update` returns at once while the charm is inactive, and the "recently eaten" prune now runs only on an E press instead of every frame. ok otherwise (no scans).
3. Allocations: fixed. `RecentKeys.Prune` reuses a buffer; `IsHeldBy` no longer copies the grabber list with `ToArray`. Debug lines that interpolate strings are guarded with a debug-mode check. No LINQ, `Find...` or new collections in per-frame code.
4. UI: not applicable, this mod touches no UI.
5. Patches: not applicable, this mod has no Harmony patches.
