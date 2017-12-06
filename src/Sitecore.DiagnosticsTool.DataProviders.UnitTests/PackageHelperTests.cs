using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;

  using Xunit;

  public class PackageHelperTests
  {
    [Theory]
    [InlineData(0, "file")]
    [InlineData(0, "file.ext")]
    [InlineData(0, "file.link.")]
    [InlineData(1, "file.link")]
    [InlineData(3, "file.link.link.link")]
    [InlineData(3, "file.ext.link.link.link")]
    [InlineData(3, "file.link.ext.link.link.link")]
    public void GetDepthTest(int depth, string name)
    {
      Assert.Equal(depth, PackageHelper.GetDepth(name));
    }    
  }
}
