using CharmGoldTooth.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmGoldTooth.Tests;

[TestClass]
public class FoodListTests
{
	[DataTestMethod]
	[DataRow("Valuable Museum Toast")]
	[DataRow("Valuable Museum Toast(Clone)")]
	[DataRow("  valuable museum toast (Clone) ")]
	[DataRow("Valuable Arctic Pills")]
	[DataRow("Valuable Wizard Red Mushroom")]
	[DataRow("Valuable Museum Gem Burger")]
	public void Default_MatchesFood(string name)
	{
		Assert.IsTrue(FoodList.Default().Matches(name));
	}

	[DataTestMethod]
	[DataRow("Valuable Museum GoldTooth")]
	[DataRow("Valuable Museum Toast Deluxe")]
	[DataRow("")]
	[DataRow("(Clone)")]
	public void Default_RejectsOthers(string name)
	{
		Assert.IsFalse(FoodList.Default().Matches(name));
	}

	[TestMethod]
	public void Matches_RejectsNull()
	{
		Assert.IsFalse(FoodList.Default().Matches(null));
	}

	[TestMethod]
	public void Parse_SplitsOnSeparatorsAndDropsBlanks()
	{
		FoodList list = FoodList.Parse(" A ,, B;C\nD , ");
		Assert.AreEqual(4, list.Count);
		Assert.IsTrue(list.Matches("c"));
		Assert.IsTrue(list.Matches("D(Clone)"));
	}

	[TestMethod]
	public void Parse_EmptyOrNullGivesEmptyList()
	{
		Assert.AreEqual(0, FoodList.Parse(null).Count);
		Assert.AreEqual(0, FoodList.Parse("  ").Count);
		Assert.IsFalse(FoodList.Parse("").Matches("Anything"));
	}

	[TestMethod]
	public void Parse_WildcardMatchesPrefix()
	{
		FoodList list = FoodList.Parse("Valuable Arctic Pills*");
		Assert.IsTrue(list.Matches("Valuable Arctic Pills"));
		Assert.IsTrue(list.Matches("Valuable Arctic Pills Big(Clone)"));
		Assert.IsFalse(list.Matches("Valuable Arctic Pill"));
		Assert.AreEqual(0, FoodList.Parse("*").Count);
	}

	[TestMethod]
	public void DefaultNames_HasEightEntries()
	{
		Assert.AreEqual(8, FoodList.Default().Count);
	}
}
