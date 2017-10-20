namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public sealed class StoredProcedure : Sitecore.Diagnostics.Database.Schema.StoredProcedure
  {
    internal StoredProcedure([NotNull] Diagnostics.SqlServer.StoredProcedure storedProcedure)
    {
      Assert.ArgumentNotNull(storedProcedure, nameof(storedProcedure));

      Header = storedProcedure.TextHeader;
      try
      {
        Body = "SQL:" + storedProcedure.TextBody;
      }
      catch (Exception)
      {
        var type = storedProcedure.ClassName + "." + storedProcedure.MethodName + ", " + storedProcedure.AssemblyName;
        Body = type.Length <= 6 ? null : "CLR:" + type;
      }
    }
  }
}