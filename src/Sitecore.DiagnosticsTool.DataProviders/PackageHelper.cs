namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System.IO;
  using System.Linq;

  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;

  public class PackageHelper
  {
    public static bool IsLegacyPackage(IFile file)
    {
      // legacy package has PackageInfo.xml in zip root
      using (var zip = file.FileSystem.ZipManager.OpenRead(file))
      {
        var fileNamesInZipRoot = zip.Entries
          .Where(x => !x.FullName.Contains('/'))
          .Select(x => x.FullName)
          .Distinct()
          .ToArray();

        return fileNamesInZipRoot.Any(x => x == "PackageInfo.xml");
      }
    }

    public static IDirectory[] ExtractMegaPackage(IFile file)
    {
      var extracted = file.ExtractZipToDirectory();
      var directories = extracted.GetDirectories();
      foreach (var directory in directories)
      {
        var links = directory.GetFiles("*.link", SearchOption.AllDirectories);
        foreach (var link in links)
        {
          TryExpandingLinkFile(link);
        }
      }

      return directories;
    }

    private static void TryExpandingLinkFile(IFile link)
    {
      var value = link.ReadAllText();
      if (!value.StartsWith("[link] "))
      {
        return;
      }

      var relativePath = value.Substring("[link] ".Length)
        .Replace("%20", " ");

      var fs = link.FileSystem;
      var path = fs.Internals.Path;
      var fullPath =
        path.GetFullPath(
          path.Combine(link.Directory.FullName, relativePath));

      var target = fs.ParseFile(fullPath);
      if (!target.Exists)
      {
        return;
      }

      link.Delete();
      link.Directory.Create();
      target.CopyTo(link.Directory, link.NameWithoutExtension);
    }
  }
}
