using CharmGoldTooth.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmGoldTooth.Tests;

[TestClass]
public class BiteHealingTests
{
	[DataTestMethod]
	[DataRow(100f, 1.5f, 2)]
	[DataRow(200f, 1.5f, 3)]
	[DataRow(300f, 1.5f, 5)]
	[DataRow(1000f, 1.5f, 15)]
	[DataRow(2500f, 1f, 25)]
	[DataRow(50f, 1.5f, 1)]
	[DataRow(1f, 0.1f, 1)]
	public void HealAmount_RoundsToNearestAndNeverBelowOne(float value, float rate, int expected)
	{
		Assert.AreEqual(expected, BiteHealing.HealAmount(value, rate));
	}

	[DataTestMethod]
	[DataRow(0f, 1.5f)]
	[DataRow(-100f, 1.5f)]
	[DataRow(500f, 0f)]
	[DataRow(500f, -1f)]
	[DataRow(float.NaN, 1.5f)]
	[DataRow(500f, float.NaN)]
	public void HealAmount_IsZeroWithoutValueOrRate(float value, float rate)
	{
		Assert.AreEqual(0, BiteHealing.HealAmount(value, rate));
	}

	[TestMethod]
	public void HealAmount_IsCapped()
	{
		Assert.AreEqual(BiteHealing.MaxHeal, BiteHealing.HealAmount(float.MaxValue, 25f));
		Assert.AreEqual(BiteHealing.MaxHeal, BiteHealing.HealAmount(float.PositiveInfinity, 25f));
	}

	[TestMethod]
	public void ShouldEat_NeedsAHealAndMissingHealthUnlessAllowed()
	{
		Assert.IsTrue(BiteHealing.ShouldEat(50, 100, false, 5));
		Assert.IsFalse(BiteHealing.ShouldEat(100, 100, false, 5));
		Assert.IsTrue(BiteHealing.ShouldEat(100, 100, true, 5));
		Assert.IsFalse(BiteHealing.ShouldEat(50, 100, true, 0));
	}

	[TestMethod]
	public void IsOn_ReadsZeroOneSetting()
	{
		Assert.IsFalse(BiteHealing.IsOn(0f));
		Assert.IsFalse(BiteHealing.IsOn(0.49f));
		Assert.IsTrue(BiteHealing.IsOn(0.5f));
		Assert.IsTrue(BiteHealing.IsOn(1f));
	}
}
