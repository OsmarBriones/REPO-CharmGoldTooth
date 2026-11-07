using BepInEx;
using CharmCore.Api;
using CharmGoldTooth.Domain;
using UnityEngine;

namespace CharmGoldTooth
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.osmar.Pockets", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.osmar.CharmCore", BepInDependency.DependencyFlags.HardDependency)]
	public class CharmGoldToothPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "com.osmar.CharmGoldTooth";
		public const string PluginName = "CharmGoldTooth";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, HealPerHundredDollars, EatAtFullHealth and the Debug section (DebugMode) in this mod config file.
			GoldToothCharm charm = new GoldToothCharm();
			CharmsApi.Register(charm, Config);

			// The food allowlist is a list of names, so it is a plain string read once here (not host-synchronised).
			string foodNames = Config.Bind<string>("Charm.GoldTooth", "FoodPrefabNames", FoodList.DefaultNames, "Names of the valuables that count as food, separated by commas. Read once when the game starts. Every player should use the same list; the host's list decides what is actually consumed. An entry ending in * matches every name that starts with it.").Value;
			charm.Food = FoodList.Parse(foodNames);
			this.gameObject.AddComponent<GoldenBite>().Charm = charm;

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}
