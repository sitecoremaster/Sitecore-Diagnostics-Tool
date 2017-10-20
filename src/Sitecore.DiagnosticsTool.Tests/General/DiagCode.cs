namespace Sitecore.DiagnosticsTool.Tests.General
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class DiagCode : Test
  {
    public override string Name { get; } = "DIAG Code";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
#pragma warning disable 618
      var code = data.System.DiagCode;
#pragma warning restore 618

      if (string.IsNullOrEmpty(code))
      {
        output.Debug("No DIAG Code provided");
        return;
      }

      var xml = new XmlDocument();
      xml.LoadXml(code);

      var elements = xml.DocumentElement?.ChildNodes.OfType<XmlElement>() ?? new XmlElement[0];
      foreach (var element in elements)
      {
        if (element == null)
        {
          continue;
        }

        switch (element.Name.ToLower())
        {
          case "setting":
            ProcessSetting(data, output, element);
            break;

          case "element":
            ProcessElement(data, output, element);
            break;
        }
      }
    }

    private static void ProcessSetting([NotNull] ITestResourceContext data, ITestOutputContext output, [NotNull] XmlElement setting)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(setting, nameof(setting));

      var name = setting.GetAttribute("name");
      Assert.IsNotNullOrEmpty(name, "name");

      var actual = data.SitecoreInfo.GetSetting(name); // null means setting is missing in showconfig

      var mode = setting.GetAttribute("mode");

      // value="some value"
      if (string.IsNullOrEmpty(mode) || mode.Equals("equals", StringComparison.OrdinalIgnoreCase))
      {
        Equals(output, setting, actual, name);
      }
      else if (mode.Equals("default", StringComparison.OrdinalIgnoreCase) || mode.Equals("isdefault", StringComparison.OrdinalIgnoreCase) || mode.Equals("defaults", StringComparison.OrdinalIgnoreCase))
      {
        Default(data, output, actual, name);
      }
      else if (mode.Equals("missing", StringComparison.OrdinalIgnoreCase) || mode.Equals("absent", StringComparison.OrdinalIgnoreCase) || mode.Equals("ismissing", StringComparison.OrdinalIgnoreCase))
      {
        Missing(output, actual, name);
      }
      else
      {
        throw new NotSupportedException(mode);
      }
    }

    private void ProcessElement([NotNull] ITestResourceContext data, ITestOutputContext output, [NotNull] XmlElement element)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(element, nameof(element));

      var xpath = element.GetAttribute("xpath");
      var mode = element.GetAttribute("mode");
      var target = data.SitecoreInfo.Configuration.SelectSingleElement(xpath);
      if (target == null)
      {
        if (mode == "missing")
        {
          output.Debug($"The \"{xpath}\" expression is not found as expected");
        }
        else
        {
          output.Error($"The \"{xpath}\" expression is not found as expected");
        }

        return;
      }

      if (mode == "exists")
      {
        output.Debug($"The \"{xpath}\" expression exists as expected");

        return;
      }

      var attributeName = element.GetAttribute("attribute").EmptyToNull() ?? element.GetAttribute("attributeName").EmptyToNull() ?? element.GetAttribute("attr");
      var expected = element.GetAttribute("expected");
      var ignoreCase = element.GetAttribute("ignoreCase");

      var comparison = ignoreCase == "false" ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
      var actual = target.GetAttribute(attributeName);
      if (string.Equals(actual, expected, comparison))
      {
        output.Debug($"The \"{xpath}\" expression value is \"{actual}\" as expected");
      }
      else
      {
        output.Error($"The \"{xpath}\" expression value is \"{actual}\" that differs from expected {expected}");
      }
    }

    private static void Equals([NotNull] ITestOutputContext output, [NotNull] XmlElement setting, string actual, string name)
    {
      Assert.ArgumentNotNull(output, nameof(output));
      Assert.ArgumentNotNull(setting, nameof(setting));

      var expected = setting.GetAttribute("value"); // missing "value" attribute means empty string
      if (!expected.Equals(actual))
      {
        output.Error($"Setting \"{name}\" value is \"{actual}\" which differs from expected \"{expected}\"");
      }
      else
      {
        output.Debug($"Setting \"{name}\" value is \"{actual}\" which equals to the expected");
      }
    }

    private static void Default(ITestResourceContext data, [NotNull] ITestOutputContext output, string actual, string name)
    {
      Assert.ArgumentNotNull(output, nameof(output));

      var expected = data.SitecoreInfo.SitecoreDefaults.GetSetting(name); // null means setting is missing in showconfig
      if (string.IsNullOrEmpty(actual))
      {
        output.Debug($"Setting \"{name}\" is missing so its value is default \"{expected}\"");
      }
      else if (actual.Equals(expected))
      {
        output.Debug($"Setting \"{name}\" value is \"{actual}\" which equals to the default");
      }
      else
      {
        output.Error($"Setting \"{name}\" value is \"{actual}\" which differs from default \"{expected}\"");
      }
    }

    private static void Missing([NotNull] ITestOutputContext output, string actual, string name)
    {
      Assert.ArgumentNotNull(output, nameof(output));

      if (actual == null)
      {
        output.Debug($"Setting \"{name}\" is absent in configuration which is expected");
      }
      else
      {
        output.Error($"Setting \"{name}\" value is \"{actual}\", however the setting must be absent in configuration");
      }
    }
  }
}