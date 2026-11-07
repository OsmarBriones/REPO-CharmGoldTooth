using System;
using System.Collections.Generic;

namespace CharmGoldTooth.Domain
{
	/// <summary>
	/// The allowlist of valuable prefab names that count as food. Built from a config string such as
	/// <c>Valuable Museum Toast, Valuable Museum Egg</c>. Matching ignores case, surrounding spaces and the "(Clone)" suffix
	/// of spawned objects. An entry ending in <c>*</c> matches every name that starts with the text before it.
	/// </summary>
	internal sealed class FoodList
	{
		/// <summary>The default list: the food tag set of the design, as the game's prefab names.</summary>
		internal const string DefaultNames = "Valuable Museum Toast, Valuable Museum Banana Bow, Valuable Wizard Red Mushroom, Valuable Museum Gumball, Valuable Museum Egg, Valuable Museum Gem Burger, Valuable Museum Milk, Valuable Arctic Pills";

		private readonly HashSet<string> exact = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly List<string> prefixes = new List<string>();

		private FoodList()
		{
		}

		internal int Count => exact.Count + prefixes.Count;

		/// <summary>Splits on commas, semicolons and line breaks; blank entries are dropped.</summary>
		internal static FoodList Parse(string? text)
		{
			FoodList list = new FoodList();
			if (string.IsNullOrWhiteSpace(text))
			{
				return list;
			}

			foreach (string raw in text!.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string entry = Normalize(raw);
				if (entry.Length == 0)
				{
					continue;
				}

				if (entry.EndsWith("*", StringComparison.Ordinal))
				{
					string prefix = entry.Substring(0, entry.Length - 1).TrimEnd();
					if (prefix.Length > 0)
					{
						list.prefixes.Add(prefix);
					}
				}
				else
				{
					list.exact.Add(entry);
				}
			}

			return list;
		}

		/// <summary>The built-in food list.</summary>
		internal static FoodList Default()
		{
			return Parse(DefaultNames);
		}

		/// <summary>True when the object name (for example <c>Valuable Museum Toast(Clone)</c>) is on the list.</summary>
		internal bool Matches(string? objectName)
		{
			string name = Normalize(objectName);
			if (name.Length == 0)
			{
				return false;
			}

			if (exact.Contains(name))
			{
				return true;
			}

			foreach (string prefix in prefixes)
			{
				if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Trims the name and removes the "(Clone)" suffix Unity adds to spawned objects.</summary>
		internal static string Normalize(string? name)
		{
			if (name == null)
			{
				return string.Empty;
			}

			string result = name.Trim();
			while (result.EndsWith("(Clone)", StringComparison.OrdinalIgnoreCase))
			{
				result = result.Substring(0, result.Length - "(Clone)".Length).TrimEnd();
			}

			return result;
		}
	}
}
