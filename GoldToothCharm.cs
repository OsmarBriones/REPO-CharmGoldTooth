using CharmCore.Api;
using CharmGoldTooth.Domain;
using Photon.Pun;
using UnityEngine;

namespace CharmGoldTooth
{
	/// <summary>
	/// Golden Bite: press E on food to eat it and heal even more. The owner heals on their own machine; the host
	/// consumes the food after validating the request (see <see cref="OnHostRequest"/>).
	/// </summary>
	internal sealed class GoldToothCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum GoldTooth";

		/// <summary>The host request that asks for a food valuable to be consumed. Argument 0 is the valuable's key (int).</summary>
		internal const string EatAction = "eat";

		internal GoldToothCharm()
			: base("GoldTooth", "Gold Tooth - Golden Bite", defaultWearPercent: 30f, defaultCooldownSeconds: null, PrefabName)
		{
			HealPerHundredDollars = AddSetting("HealPerHundredDollars", 1.5f, 0f, 25f, "Health points one bite heals for every $100 of the food's current value (rounded to a whole number, at least 1). 0 means eating heals nothing, so the bite is refused.");
			EatAtFullHealth = AddSetting("EatAtFullHealth", 0f, 0f, 1f, "0 = you cannot eat while your health is full (the food is kept), 1 = you can eat at full health and the food is wasted.");
		}

		/// <summary>Health points healed per $100 of the food's current value (0 to 25).</summary>
		internal CharmSetting HealPerHundredDollars { get; }

		/// <summary>0 or 1: whether eating at full health is allowed.</summary>
		internal CharmSetting EatAtFullHealth { get; }

		/// <summary>The food allowlist, read once at startup from the plugin's config string.</summary>
		internal FoodList Food { get; set; } = FoodList.Default();

		/// <summary>Valuables the local player has just eaten (short-lived), so a second E press cannot heal twice.</summary>
		internal RecentKeys Recent { get; } = new RecentKeys();

		protected override void OnDeactivated()
		{
			Recent.Clear();
		}

		/// <summary>
		/// Host only: checks the sender really holds a food valuable that is not in a pocket, then destroys it.
		/// (<c>PhysGrabObject.DestroyPhysGrabObject</c> only acts on the master client.)
		/// </summary>
		protected override void OnHostRequest(PlayerAvatar sender, string action, object[] args)
		{
			if (action != EatAction)
			{
				LogDebug($"Host: ignoring unknown request '{action}'");
				return;
			}

			if (args == null || args.Length < 1 || !(args[0] is int key) || !ValuableKey.IsUsable(SemiFunc.IsMultiplayer(), key))
			{
				LogDebug("Host: eat request without a usable valuable key; ignored");
				return;
			}

			PhysGrabObject? physGrabObject = Resolve(key);
			ValuableObject? valuable = physGrabObject != null ? physGrabObject.GetComponent<ValuableObject>() : null;
			if (physGrabObject == null || valuable == null)
			{
				LogDebug($"Host: eat request for key {key}: no such valuable (already gone?); ignored");
				return;
			}

			if (!Food.Matches(valuable.name))
			{
				LogDebug($"Host: refused {valuable.name} (key {key}): not on the food list");
				return;
			}

			if (!IsHeldBy(physGrabObject, sender))
			{
				LogDebug($"Host: refused {valuable.name} (key {key}): the sender is not holding it");
				return;
			}

			if (CharmsApi.CarrierOf(valuable) != null)
			{
				LogDebug($"Host: refused {valuable.name} (key {key}): it is in a pocket");
				return;
			}

			LogDebug($"Host: consuming {valuable.name} (key {key}, value {valuable.dollarValueCurrent:0}) eaten by its holder");
			physGrabObject.DestroyPhysGrabObject();
		}

		private static bool IsHeldBy(PhysGrabObject physGrabObject, PlayerAvatar sender)
		{
			foreach (PhysGrabber grabber in physGrabObject.playerGrabbing.ToArray())
			{
				if (grabber != null && grabber.playerAvatar == sender)
				{
					return true;
				}
			}

			return false;
		}

		private static PhysGrabObject? Resolve(int key)
		{
			if (SemiFunc.IsMultiplayer())
			{
				PhotonView? view = PhotonView.Find(key);
				return view != null ? view.GetComponent<PhysGrabObject>() : null;
			}

			foreach (PhysGrabObject candidate in Object.FindObjectsByType<PhysGrabObject>(FindObjectsSortMode.None))
			{
				if (candidate.gameObject.GetInstanceID() == key)
				{
					return candidate;
				}
			}

			return null;
		}
	}
}
