namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  public static class TestContextHelper
  {
    [NotNull]
    public static IEnumerable<PropertyInfo> GetContextResourceProperties()
    {
      return typeof(InstanceResourceContext).GetProperties().Where(x => x.PropertyType.GetInterfaces().Any(y => y == typeof(IResource)));
    }
  }
}