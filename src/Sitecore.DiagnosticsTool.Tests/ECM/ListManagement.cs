namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.General.Health;
  using Sitecore.DiagnosticsTool.Core.Collections;

  public class ListManagement : Test
  {
    public override IEnumerable<Category> Categories => new[] { Category.Ecm };
    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      IList<string> filesToBeDisabled = new List<string>();
      Assert.ArgumentNotNull(data, nameof(data));
      foreach (var filePath in data.SitecoreInfo.IncludeFiles.Keys)
      {
        if (filePath.StartsWith("\\App_Config\\Include\\ListManagement\\",StringComparison.InvariantCultureIgnoreCase)&&filePath.Contains("Sitecore.ListManagement"))
        {
          filesToBeDisabled.Add(filePath);
        }
      }
      if (filesToBeDisabled.Count > 0)
      {
        output.Warning(ErrorMessage(),
          detailed: GetMessage(filesToBeDisabled));
      }
    }

    public override string Name => "ListManagement is disabled on CD";


    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major == 8;
    }
    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }

    protected ShortMessage ErrorMessage()
    {
      return "One or several files in App_Cofig\\Include\\ListManagement\\ folder should be disabled ";
    }
    protected DetailedMessage GetMessage(IList<string> filesToBeDisabled)
    {
      var rows = filesToBeDisabled.Select(d =>
          new TableRow(
            new Pair("File", $"{d}"),
            new Pair("Comment", "Should be disabled")))
          .ToArray();

      var message = new DetailedMessage(
        new Table(rows));

      return message;
    }
  }
}