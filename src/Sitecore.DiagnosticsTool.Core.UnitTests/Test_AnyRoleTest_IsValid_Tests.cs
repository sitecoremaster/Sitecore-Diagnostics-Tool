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

  public class Test_AnyRoleTest_IsValid_Tests
  {
    [Fact]
    public void AnyRoleTest_ContentDelivery_IsValid_True()
    {
      var sut = new AnyRoleTest();
      var ver = new SitecoreVersion(8, 0, 0, 141212);

      sut.IsActual(new[] { ServerRole.ContentDelivery }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    [Fact]
    public void AnyRoleTest_ContentManagement_IsValid_True()
    {
      var sut = new AnyRoleTest();
      var ver = new SitecoreVersion(8, 0, 0, 141212);

      sut.IsActual(new[] { ServerRole.ContentManagement }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    [Fact]
    public void AnyRoleTest_Processing_IsValid_True()
    {
      var sut = new AnyRoleTest();
      var ver = new SitecoreVersion(8, 0, 0, 141212);

      sut.IsActual(new[] { ServerRole.Processing, }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    [Fact]
    public void AnyRoleTest_ContentManagement_Processing_IsValid_True()
    {
      var sut = new AnyRoleTest();
      var ver = new SitecoreVersion(8, 0, 0, 141212);

      sut.IsActual(new[] { ServerRole.ContentManagement, ServerRole.Processing, }, ver, new TestResourceContext(""))
        .Should()
        .BeTrue();
    }

    private class AnyRoleTest : Test
    {
      public override string Name { get; } = nameof(ServerRoles);

      public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        throw new NotImplementedException();
      }
    }
  }
}