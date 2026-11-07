# QA battery: Golden Bite (CharmGoldTooth)

Deploy: Pockets, CharmCore and CharmGoldTooth are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmGoldTooth.cfg` (off by default). When a level starts, a Gold Tooth is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. Any level works.

## Single player
- [ ] Log shows `Charm registered: Gold Tooth - Golden Bite` and `CharmGoldTooth 1.0.0 loaded!`.
- [ ] Store the valuable in a slot: the slot shows its picture and value.
- [ ] Hurt yourself first (a fall or a monster) so health is below the maximum. With `DebugMode` on, spawn or find a food valuable (Toast, Egg, Milk, Pills...).
- [ ] Hold the food and press E: it disappears, health rises by about 1.5 HP per $100 of its value (log: `Golden Bite: healed N`), the heal flash shows, and the Gold Tooth value drops by about 30% of its original value.
- [ ] Press E several times quickly on one food: only one heal and one wear (log: `was just eaten; ignored`).
- [ ] Press E while holding a non-food valuable (for example a vase): nothing happens, no wear (log: `not on the food list`).
- [ ] Press E holding nothing: nothing happens.
- [ ] At full health with `EatAtFullHealth = 0`: nothing happens, the food stays in your hands, no wear. With `1`: the food is eaten, no heal, the Gold Tooth wears.
- [ ] Try a damaged food of very low value: it still heals 1 (the minimum).
- [ ] No cooldown veil appears on the slot.
- [ ] With the Gold Tooth out of the pocket, pressing E on food does nothing.
- [ ] Take the valuable out: the power stops.
- [ ] Use it until the valuable breaks: the slot empties, the power stops, no errors in the log.

## Settings
- [ ] `WearPercent = 0`: food heals, no wear. `HealPerHundredDollars = 0`: eating is refused. `10`: a $100 food heals 10.
- [ ] `FoodPrefabNames = Valuable Museum Toast` (restart): only Toast works. Empty value: nothing counts as food.
- [ ] Charm `Enabled = false`: no effect and no wear. CharmCore `Enabled = false`: nothing works.

## Multiplayer (host and client)
- [ ] On a client: holding food and pressing E heals the client, the food disappears for everyone (also on the host), and the Gold Tooth value drops on both machines.
- [ ] The host eating works the same; a client holding food that the host's list excludes heals locally but the food is not consumed (known limit: keep the lists equal).
- [ ] A friend without the Gold Tooth cannot eat; the friend's own food is unaffected.
- [ ] Host settings override the client's.
- [ ] Client leaves and rejoins: no stuck veil.

Log patterns: `Charm registered`, `Golden Bite: healed`, `Host: consuming`, `Host: refused`, `threw`, `Exception`.
