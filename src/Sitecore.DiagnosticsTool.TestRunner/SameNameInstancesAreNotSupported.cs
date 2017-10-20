namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public class SameNameInstancesAreNotSupported : NotSupportedException
  {
    public SameNameInstancesAreNotSupported([NotNull] string name)
      : base($"Several same-name instances are not supported: {name}")
    {
      Assert.ArgumentNotNullOrEmpty(name);
    }
  }
}