# Changelog

## [Unreleased]

### Changed
- Runtime hygiene pass: no static game references, no scene-wide search, level state cleared when the level ends, fewer allocations and less per-frame work. No behaviour change.

### Added
- Golden Bite: while the Gold Tooth is in the pocket, pressing E on a held food valuable eats it (the food is consumed) and heals 1.5 HP per $100 of its current value; the Gold Tooth wears 30% per bite.
- Settings `HealPerHundredDollars` (0 to 25), `EatAtFullHealth` (0 or 1) and the text entry `FoodPrefabNames` (default: Toast, Banana Bow, Red Mushroom, Gumball, Egg, Gem Burger, Milk, Pills).
- `Debug.DebugMode` (off by default): puts the Gold Tooth on the floor at the start of each level and writes detailed logs.
