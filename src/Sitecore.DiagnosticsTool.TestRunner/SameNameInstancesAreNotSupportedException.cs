namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public class SameNameInstancesAreNotSupportedException : NotSupportedException
  {
    public SameNameInstancesAreNotSupportedException([NotNull] string name)
      : base($"Several same-name instances are not supported: {name}")
    {
      Assert.ArgumentNotNullOrEmpty(name);
    }
  }
}