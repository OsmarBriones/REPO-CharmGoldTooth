using CharmCore.Api;

namespace CharmGoldTooth
{
	/// <summary>Golden Bite: press E on food to eat it and heal even more.</summary>
	internal sealed class GoldToothCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum GoldTooth";

		internal GoldToothCharm()
			: base("GoldTooth", "Gold Tooth - Golden Bite", defaultWearPercent: 30f, defaultCooldownSeconds: null, PrefabName)
		{
			// Extra host-synced numbers for this power go here, for example:
			// Amount = AddSetting("Amount", 30f, 1f, 100f, "Description for the config file.");
		}
	}
}
