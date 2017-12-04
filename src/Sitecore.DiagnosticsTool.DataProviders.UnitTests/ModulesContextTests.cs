namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using System.Linq;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  using Xunit;

  public class ModulesContextTests
  {
    [Fact]
    public void Test()
    {
      // arrange
      var assemblies = new[]
      {
        // SC 8.2.1
        new AssemblyFile("Antlr3.Runtime.dll", "3.5.0.2", "3.5.0.2"),

        // SPIF 2.1 rev. 150128
        new AssemblyFile("Sitecore.Sharepoint.Common.dll", "2.1.5049", " 2.1 rev. 150128", 37376),
        new AssemblyFile("Sitecore.Sharepoint.Data.Providers.dll", "2.1.5049", " 2.1 rev. 150128", 72704),
        new AssemblyFile("Sitecore.Sharepoint.Data.WebServices.dll", "2.1.5049", " 2.1 rev. 150128"),
        new AssemblyFile("Sitecore.Sharepoint.Installer.dll", "2.1.5049", " 2.1 rev. 150128", 17408),
        new AssemblyFile("Sitecore.Sharepoint.ObjectModel.dll", "2.1.5049", " 2.1 rev. 150128", 65024),
        new AssemblyFile("Sitecore.Sharepoint.Web.dll", "2.1.5049", " 2.1 rev. 150128", 73728),

        // WFFM 
        new AssemblyFile("Sitecore.Forms.Mvc.dll", "8.2.5600.0", "8.2 rev. 160801"),
      };

      var assembliesMap = assemblies.ToDictionary(x => x.FileName, x => x);

      var sut = new ModulesContext(assembliesMap);

      // act
      var correct = sut.InstalledModules;
      var correctModule = correct.First();
      var incorrect = sut.IncorrectlyInstalledModules;
      var incorrectModule = incorrect.First();

      // assert
      Assert.Equal("SharePoint Integration Framework", correctModule.Key);
      Assert.Equal("2.1", correctModule.Value?.Release?.Version.MajorMinor);
      Assert.Equal("150128", correctModule.Value?.Release?.Revision);

      Assert.Equal("Web Forms for Marketers", incorrectModule.Key);
      Assert.Equal("8.2", incorrectModule.Value.Single()?.Release?.Version.MajorMinor);
      Assert.Equal("160801", incorrectModule.Value.Single()?.Release?.Revision);

      Assert.Equal(1, correct.Count);
      Assert.Equal(1, incorrect.Count);
    }
  }
}