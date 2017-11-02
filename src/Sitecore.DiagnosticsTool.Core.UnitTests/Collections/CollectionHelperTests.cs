namespace Sitecore.DiagnosticsTool.Core.UnitTests.Collections
{
  using System;
  using System.Collections.Generic;

  using Sitecore.DiagnosticsTool.Core.Collections;

  using Xunit;

  public class CollectionHelperTests
  {
    [Theory]
    [InlineData(true, new[] { 0 }, new[] { 0 })]
    [InlineData(true, new[] { 0, 1 }, new[] { 0, 1 })]
    [InlineData(false, new[] { 0, 1 }, new[] { 0 })]
    [InlineData(false, new[] { 0, 1 }, new[] { 1, 0 })]
    [InlineData(false, new[] { 0, 1 }, new[] { 0, 2 })]
    public void AreIdenticalTest(bool expected, int[] first, int[] second)
    {
      Assert.Equal(expected, CollectionHelper.AreIdentical(first, second, (a, b) => a == b));
      Assert.Equal(expected, CollectionHelper.AreIdentical(second, first, (a, b) => a == b));
    }

    [Theory]
    [InlineData(true, new[] { "0", "zero" }, new[] { "0", "zero" })]
    [InlineData(true, new[] { "0", "zero", "1", "one" }, new[] { "0", "zero", "1", "one" })]
    [InlineData(true, new[] { "0", "zero", "1", "one" }, new[] { "1", "one", "0", "zero" })]
    [InlineData(false, new[] { "0", "zero", "1", "one" }, new[] { "0", "zero" })]
    [InlineData(false, new[] { "0", "zero", "1", "one" }, new[] { "0", "zero", "1", "ONE!" })]
    [InlineData(false, new[] { "0", "zero", "1", "one" }, new[] { "0", "zero", "-1", "one" })]
    [InlineData(false, new[] { "0", "zero", "1", "one" }, new[] { "2", "zero", "3", "one" })]
    public void AreIdenticalTest(bool expected, string[] first, string[] second)
    {
      var dictA = new Dictionary<int, string>();
      for (var i = 0; i < first.Length; i += 2)
      {
        dictA.Add(int.Parse(first[i]), first[i + 1]);
      }

      var dictB = new Dictionary<int, string>();
      for (var i = 0; i < second.Length; i += 2)
      {
        dictB.Add(int.Parse(second[i]), second[i + 1]);
      }

      Assert.Equal(expected, CollectionHelper.AreIdentical(dictA, dictB, (a, b) => a == b, (a, b) => string.Equals(a, b, StringComparison.Ordinal)));
      Assert.Equal(expected, CollectionHelper.AreIdentical(dictB, dictA, (a, b) => a == b, (a, b) => string.Equals(a, b, StringComparison.Ordinal)));
    }

    [Theory]
    [InlineData(true, new[] { "A", "A" })]
    [InlineData(true, new[] { "A", "A", "A" })]
    [InlineData(false, new[] { "A", "A", "B" })]
    public void AreIdenticalByPairsTest(bool expected, string[] collection)
    {
      Assert.Equal(expected, CollectionHelper.AreIdenticalByPairs(collection, (a, b) => string.Equals(a, b, StringComparison.Ordinal)));
      Assert.Equal(expected, CollectionHelper.AreIdenticalByPairs(collection, (a, b) => string.Equals(a, b, StringComparison.Ordinal)));
    }
  }
}