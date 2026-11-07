using System;

namespace CharmGoldTooth.Domain
{
	/// <summary>Game-free rules of Golden Bite: how much a bite heals and whether it may happen. Free of Unity and game types so it is unit tested.</summary>
	internal static class BiteHealing
	{
		/// <summary>Most health points a single bite can ask for; the game clamps to the player's maximum anyway.</summary>
		internal const int MaxHeal = 10000;

		/// <summary>
		/// Health points a bite gives: <paramref name="hpPerHundredDollars"/> for every $100 of <paramref name="dollarValue"/>,
		/// rounded to the nearest whole point (halves go up), and at least 1 when both numbers are positive.
		/// Returns 0 when the value or the rate is not positive (or not a number).
		/// </summary>
		internal static int HealAmount(float dollarValue, float hpPerHundredDollars)
		{
			if (float.IsNaN(dollarValue) || float.IsNaN(hpPerHundredDollars) || dollarValue <= 0f || hpPerHundredDollars <= 0f)
			{
				return 0;
			}

			double raw = (double)dollarValue / 100.0 * hpPerHundredDollars;
			if (raw >= MaxHeal)
			{
				return MaxHeal;
			}

			int amount = (int)Math.Round(raw, MidpointRounding.AwayFromZero);
			return amount < 1 ? 1 : amount;
		}

		/// <summary>
		/// True when a bite should go ahead: it would heal at least 1 point, and the player is hurt
		/// (or the host allows eating at full health).
		/// </summary>
		internal static bool ShouldEat(int health, int maxHealth, bool eatAtFullHealth, int healAmount)
		{
			if (healAmount < 1)
			{
				return false;
			}

			return health < maxHealth || eatAtFullHealth;
		}

		/// <summary>Reads a 0/1 setting stored as a float: 0.5 and above is on.</summary>
		internal static bool IsOn(float setting)
		{
			return setting >= 0.5f;
		}
	}
}
