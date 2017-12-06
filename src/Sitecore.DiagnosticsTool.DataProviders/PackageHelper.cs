namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System;
  using System.IO;
  using System.Linq;

  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Core.Extensions;

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

      // workaround for #197008 - we need to expand .link.link.link before .link.link and before .link
      var depth = extracted.GetFiles("*.link", SearchOption.AllDirectories).Max(x => GetDepth(x.Name));
      for (var i = depth; i > 0; --i)
      {
        var suffix = Enumerable.Repeat(".link", i).JoinToString("");

        foreach (var directory in directories)
        {
          var links = directory.GetFiles("*" + suffix, SearchOption.AllDirectories);
          foreach (var link in links)
          {
            TryExpandingLinkFile(link);
          }
        }
      }

      return directories;
    }

    public static int GetDepth(string name)
    {
      var count = 0;
      var shift = name.Length;
      while (true)
      {
        if (!EndsWith(name, ".link", shift))
        {
          break;
        }

        shift -= ".link".Length;
        count++;
      }

      return count;
    }

    private static bool EndsWith(string name, string suffix, int maxLength)
    {
      return maxLength - suffix.Length >= 0 && name.IndexOf(suffix, maxLength - suffix.Length) == maxLength - suffix.Length;
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
