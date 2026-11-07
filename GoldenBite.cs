using CharmGoldTooth.Domain;
using UnityEngine;

namespace CharmGoldTooth
{
	/// <summary>
	/// Golden Bite, owner side. While the Gold Tooth is pocketed, pressing E (Interact) while holding a food valuable
	/// heals the local player (health is owned by each player's own machine) and asks the host to consume the food.
	/// </summary>
	internal sealed class GoldenBite : MonoBehaviour
	{
		/// <summary>How long a valuable that was just eaten is remembered, in seconds.</summary>
		private const float RememberSeconds = 3f;

		internal GoldToothCharm Charm { get; set; } = null!;

		private void Update()
		{
			if (Charm == null || !Charm.IsActive)
			{
				return;
			}

			if (!SemiFunc.InputDown(InputKey.Interact))
			{
				return;
			}

			Charm.Recent.Prune(Time.time);
			TryEat();
		}

		private void TryEat()
		{
			PhysGrabber? grabber = PhysGrabber.instance;
			if (grabber == null || !grabber.grabbed || grabber.grabbedPhysGrabObject == null)
			{
				Charm.LogDebug("E pressed: not holding anything");
				return;
			}

			PhysGrabObject physGrabObject = grabber.grabbedPhysGrabObject;
			ValuableObject? valuable = physGrabObject.GetComponent<ValuableObject>();
			if (valuable == null)
			{
				if (Charm.DebugMode) { Charm.LogDebug($"E pressed: {physGrabObject.name} is not a valuable"); }
				return;
			}

			if (!Charm.Food.Matches(valuable.name))
			{
				if (Charm.DebugMode) { Charm.LogDebug($"E pressed: {valuable.name} is not on the food list"); }
				return;
			}

			PlayerAvatar? avatar = PlayerAvatar.instance;
			PlayerHealth? health = avatar != null ? avatar.playerHealth : null;
			if (avatar == null || health == null || avatar.deadSet || avatar.isDisabled)
			{
				Charm.LogDebug("E pressed: the player is dead or disabled");
				return;
			}

			int viewId = physGrabObject.photonView != null ? physGrabObject.photonView.ViewID : 0;
			int key = ValuableKey.Of(SemiFunc.IsMultiplayer(), viewId, physGrabObject.gameObject.GetInstanceID());
			if (Charm.Recent.Contains(key, Time.time))
			{
				if (Charm.DebugMode) { Charm.LogDebug($"E pressed: {valuable.name} (key {key}) was just eaten; ignored"); }
				return;
			}

			float value = valuable.dollarValueCurrent;
			int amount = BiteHealing.HealAmount(value, Charm.HealPerHundredDollars.Value);
			bool atFullHealth = BiteHealing.IsOn(Charm.EatAtFullHealth.Value);
			if (Charm.DebugMode) { Charm.LogDebug($"E pressed on {valuable.name} (key {key}): value {value:0}, heal {amount}, health {health.health}/{health.maxHealth}, eat at full health {atFullHealth}"); }
			if (!BiteHealing.ShouldEat(health.health, health.maxHealth, atFullHealth, amount))
			{
				Charm.LogDebug("Bite refused: nothing to heal, or health is full");
				return;
			}

			Charm.Recent.TryAdd(key, Time.time, RememberSeconds);
			if (!Charm.TryUse())
			{
				Charm.Recent.Remove(key);
				Charm.LogDebug("Bite refused: the charm is not ready");
				return;
			}

			health.Heal(amount, true);
			if (Charm.DebugMode) { Charm.LogDebug($"Golden Bite: healed {amount} (health now {health.health}/{health.maxHealth}); asking the host to consume key {key}"); }
			Charm.SendToHost(GoldToothCharm.EatAction, key);
		}
	}
}
