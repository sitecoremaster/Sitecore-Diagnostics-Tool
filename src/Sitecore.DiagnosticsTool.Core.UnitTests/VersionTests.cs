namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using FluentAssertions;

  using Sitecore.Diagnostics.Objects;

  using Xunit;

  public class VersionTests
  {
    [Fact]
    public void ToStringTest()
    {
      new SitecoreVersion(4, 3, 2, 111111).ToString()
        .Should().Be("4.3 Update-2 (rev. 111111)");
    }

    [Fact]
    public void ConstructurConsistencyTest()
    {
      var actual = new SitecoreVersion(4, 3, 2, 111111);

      actual.Major.Should().Be(4);
      actual.Minor.Should().Be(3);
      actual.Update.Should().Be(2);
      actual.Revision.Should().Be(111111);
    }

    [Fact]
    public void ComparerTest()
    {
      new SitecoreVersion(2, 3, 4, 5).Equals(null).Should().BeFalse();

      // ReSharper disable once EqualExpressionComparison
      new SitecoreVersion(2, 3, 4, 5).Equals(new SitecoreVersion(2, 3, 4, 5)).Should().BeTrue();
      new SitecoreVersion(2, 3, 4, 5).Equals(new SitecoreVersion(2, 3, 4, 0)).Should().BeFalse();
      new SitecoreVersion(2, 3, 4, 5).Equals(new SitecoreVersion(2, 4, 4, 5)).Should().BeFalse();
      new SitecoreVersion(2, 3, 4, 5).Equals(new SitecoreVersion(1, 3, 4, 5)).Should().BeFalse();

      // ReSharper disable once EqualExpressionComparison                              
      new SitecoreVersion(2, 3, 4, 5, "Hotfix 123456-2").Equals(new SitecoreVersion(2, 3, 4, 5, "Hotfix 123456-2")).Should().BeTrue();
      new SitecoreVersion(2, 3, 4, 5, "Hotfix 123456-2").Equals(new SitecoreVersion(2, 3, 4, 5, "Hotfix 123456")).Should().BeFalse();
      new SitecoreVersion(2, 3, 4, 5, "Hotfix 123456-2").Equals(new SitecoreVersion(2, 3, 4, 5)).Should().BeFalse();
    }
  }
}