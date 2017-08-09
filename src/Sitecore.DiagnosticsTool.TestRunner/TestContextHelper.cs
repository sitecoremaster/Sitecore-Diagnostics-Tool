namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public static class TestContextHelper
  {
    [NotNull]
    public static IEnumerable<PropertyInfo> GetContextResourceProperties()
    {
      return typeof(TestResourceContext).GetProperties().Where(x => x.PropertyType.GetInterfaces().Any(y => y == typeof(IResource)));
    }

    [NotNull]
    public static IEnumerable<PropertyInfo> GetContextIResourceProperties()
    {
      return typeof(ITestResourceContext).GetProperties().Where(x => x.PropertyType.GetInterfaces().Any(y => y == typeof(IResource)));
    }
  }
}