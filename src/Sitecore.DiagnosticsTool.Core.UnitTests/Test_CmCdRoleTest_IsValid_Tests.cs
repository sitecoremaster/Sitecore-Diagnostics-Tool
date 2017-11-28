namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using System;
  using System.Collections.Generic;

  using FluentAssertions;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.TestRunner;

  using Xunit;

  public class Test_CmCdRoleTest_IsValid_Tests
  {
    [Fact]
    public void CmCdRoleTest_ContentDelivery_IsValid_True()
    {
      var sut = new CmCdRoleTest();
      var ver = new SitecoreVersion(8, 0, 0);

      sut.IsActual(new[] { ServerRole.ContentDelivery }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    [Fact]
    public void CmCdRoleTest_ContentMangement_IsValid_True()
    {
      var sut = new CmCdRoleTest();
      var ver = new SitecoreVersion(8, 0, 0);

      sut.IsActual(new[] { ServerRole.ContentManagement }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    [Fact]
    public void CmCdRoleTest_Processing_IsValid_False()
    {
      var sut = new CmCdRoleTest();
      var ver = new SitecoreVersion(8, 0, 0);

      sut.IsActual(new[] { ServerRole.Processing, }, ver, new TestResourceContext(""))
        .Should()
        .BeFalse();
    }

    [Fact]
    public void CmCdRoleTest_ContentManagement_Processing_IsValid_True()
    {
      var sut = new CmCdRoleTest();
      var ver = new SitecoreVersion(8, 0, 0);

      sut.IsActual(new[] { ServerRole.ContentManagement, ServerRole.Processing, }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    private class CmCdRoleTest : Test
    {
      public override string Name { get; } = nameof(ServerRoles);

      public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

      public override IEnumerable<ServerRole> ServerRoles { get; } = new[] { ServerRole.ContentManagement, ServerRole.ContentDelivery };

      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        throw new NotImplementedException();
      }
    }
  }
}