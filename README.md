# Charm Gold Tooth

Golden Bite: while the Gold Tooth is in your pocket, press E while holding food to eat it and heal. The pricier the food, the more you heal.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Gold Tooth** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. Pick up a food valuable (Toast, Banana Bow, Red Mushroom, Gumball, Egg, Gem Burger, Milk or Pills) and, while holding it, press **E**. You eat it: it is gone and you heal 1.5 health points for every $100 it is worth right now.
3. Take the valuable out of the pocket and the power stops.

## Features

- **Golden Bite:** eating food heals you based on its current value (a damaged piece of food heals less). If you are already at full health nothing happens and the food is kept. Only you are healed; the food is consumed for everybody.
- **It costs the Gold Tooth:** each bite wears away 30% of its original value, and a worn-out Gold Tooth breaks. The food you eat is lost completely.
- **Everything is configurable:** wear, how much each $100 heals, whether you may eat at full health, and which valuables count as food. The host's choices apply to everyone.

## Requirements
- [BepInEx Pack for R.E.P.O.](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
- [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/)
- [CharmCore](https://thunderstore.io/c/repo/p/OsmarBriones/CharmCore/)

## Installation
1. Install through r2modman / Thunderstore Mod Manager (Pockets and CharmCore come with it), or place `CharmGoldTooth.dll` next to `Pockets.dll` and `CharmCore.dll` in `BepInEx/plugins`. **All players need it.**
2. Launch the game once; the configuration file is created inside `BepInEx/config`.

## Configuration
Settings are in `com.osmar.CharmGoldTooth.cfg` inside `BepInEx/config`, section `Charm.GoldTooth`. The host's choices apply to everyone.

- `Enabled` (default `true`): turn this charm on or off.
- `WearPercent` (default `30`, 0 to 100): share of the Gold Tooth's original value lost on each bite.
- `HealPerHundredDollars` (default `1.5`, 0 to 25): health points healed for every $100 of the food's current value, rounded to a whole number (at least 1). `0` makes eating pointless, so the bite is refused.
- `EatAtFullHealth` (default `0`, 0 or 1): `1` lets you eat when your health is full (the food is wasted).
- `FoodPrefabNames` (text, same section): the valuables that count as food, separated by commas. Read once when the game starts, so restart after changing it; every player should use the same list, and the host's list decides what is really consumed. A name ending in `*` matches every name that starts with it. Default: Toast, Banana Bow, Red Mushroom, Gumball, Egg, Gem Burger, Milk and Pills.

Section `Debug`: `DebugMode` (default `false`) places a Gold Tooth on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing. `SpawnNow` (default `false`, needs `DebugMode`): set it to `true` while a level is running to place one right away; it switches itself back off.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmGoldTooth/issues)

## Credits
Developed by **Osmar Briones**
