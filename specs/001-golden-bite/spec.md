## User Scenarios

### User Story 1 - Golden Bite (Priority: P1)
A player stores the Gold Tooth in a pocket and gets this power while it stays there: press E while holding a food valuable to eat it and heal 1.5 HP per $100 of its current value.
**Acceptance**: with the valuable pocketed the power works; with it out of the pocket it does not; each bite consumes the food and wears the Gold Tooth by 30%.

## Requirements
- FR-001: Registers a charm with CharmCore for the prefab Valuable Museum GoldTooth.
- FR-002: Settings through CharmCore: Enabled, WearPercent, HealPerHundredDollars, EatAtFullHealth, DebugMode (plus the local FoodPrefabNames list); host-authoritative except DebugMode.
- FR-003: All players install it (Category 2); depends on Pockets and CharmCore.