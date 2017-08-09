namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class FieldsWithSpaces : KbTest
  {
    public override string KbNumber => "036144";

    public override string KbName { get; } = "Issues with the Lucene search provider and Sitecore fields that have spaces in the names";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 7;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var fieldsConfigIgnored = new HashSet<string>();
      var fieldsAnalyzerIgnored = new HashSet<string>();

      var config = data.SitecoreInfo.Configuration;
      var sitecore = config.SelectSingleNode("/configuration/sitecore");
      if (sitecore == null)
      {
        return;
      }
      
      var defaultIndexConfigurationPath = data.SitecoreInfo.GetSetting("ContentSearch.DefaultIndexConfigurationPath");
      var defaultFieldMapPath = defaultIndexConfigurationPath + "/fieldMap";
      var defaultFieldNames = ExtractFieldNames(config.SelectElements(@"/configuration/sitecore/" + defaultFieldMapPath).FirstOrDefault());

      var defaultFieldNamesAreVerified = false;
      var indexes = config.SelectElements(@"/configuration/sitecore/contentSearch/configuration/indexes/*");
      foreach (var index in indexes)
      {
        if (!index.HasAttribute("type") || !index.GetAttribute("type").Contains("Lucene"))
        {
          continue;
        }

        var configuration = index.SelectElements(@"configuration").FirstOrDefault();
        if (configuration == null)
        {
          if (defaultFieldNamesAreVerified)
          {
            continue;
          }

          VerifyFieldNames(defaultFieldNames, fieldsConfigIgnored, fieldsAnalyzerIgnored);
          defaultFieldNamesAreVerified = true;
        }
        else
        {
          var fieldMap = configuration.SelectElements("fieldMap").FirstOrDefault();
          if (fieldMap == null)
          {
            if (defaultFieldNamesAreVerified)
            {
              continue;
            }

            VerifyFieldNames(defaultFieldNames, fieldsConfigIgnored, fieldsAnalyzerIgnored);
            defaultFieldNamesAreVerified = true;
          }
          else
          {
            if (fieldMap.HasAttribute("ref"))
            {
              var referencedFieldMapPath = @"/configuration/sitecore/" + fieldMap.GetAttribute("ref");

              Map<XmlElement> mergedFieldNames;
              if (referencedFieldMapPath.Equals(defaultFieldMapPath, StringComparison.InvariantCulture))
              {
                mergedFieldNames = ExtractFieldNames(fieldMap).Concat(defaultFieldNames).ToMap(kvp => kvp.Key, kvp => kvp.Value);
              }
              else
              {
                var referencedFieldMap = config.SelectElements(referencedFieldMapPath).FirstOrDefault();
                mergedFieldNames = ExtractFieldNames(fieldMap).Concat(ExtractFieldNames(referencedFieldMap)).ToMap(kvp => kvp.Key, kvp => kvp.Value);
              }

              VerifyFieldNames(mergedFieldNames, fieldsConfigIgnored, fieldsAnalyzerIgnored);
            }
            else
            {
              var fieldNames = ExtractFieldNames(fieldMap);
              VerifyFieldNames(fieldNames, fieldsConfigIgnored, fieldsAnalyzerIgnored);
            }
          }
        }
      }

      if (fieldsConfigIgnored.Count > 0)
      {
        output.Warning("Configuration of the following field(s) is ignored:\r\n" + new BulletedList(fieldsConfigIgnored) + "\r\n", url: Link);
      }

      if (fieldsAnalyzerIgnored.Count > 0)
      {
        output.Warning("Analyzer of the following field(s) is ignored:\r\n" + new BulletedList(fieldsAnalyzerIgnored), url: Link);
      }
    }

    protected static void VerifyFieldNames([NotNull] Map<XmlElement> fieldNames, [NotNull] HashSet<string> fieldsWhoseConfigurationIsIgnored, [NotNull] HashSet<string> fieldsWhoseAnalyzerIsIgnored)
    {
      Assert.ArgumentNotNull(fieldNames, nameof(fieldNames));
      Assert.ArgumentNotNull(fieldsWhoseConfigurationIsIgnored, nameof(fieldsWhoseConfigurationIsIgnored));
      Assert.ArgumentNotNull(fieldsWhoseAnalyzerIsIgnored, nameof(fieldsWhoseAnalyzerIsIgnored));

      var names = fieldNames.Keys;
      foreach (var fieldName in fieldNames.Where(fieldName => !fieldName.Key.StartsWith("_") && fieldName.Key != "isbucket_text"))
      {
        var name = fieldName.Key;
        if (name.Contains("_"))
        {
          if (!names.Contains(name.Replace("_", " ")))
          {
            fieldsWhoseConfigurationIsIgnored.Add(name);
          }

          continue;
        }

        if (!name.Contains(" "))
        {
          continue;
        }

        if (names.Contains(name.Replace(" ", "_")) || fieldName.Value.FirstChild == null)
        {
          continue;
        }

        fieldsWhoseAnalyzerIsIgnored.Add(name);
      }
    }

    [NotNull]
    protected static Map<XmlElement> ExtractFieldNames(XmlElement fieldMap)
    {
      return fieldMap?.SelectElements("fieldNames/*").Select(x => new KeyValuePair<string, XmlElement>(x.Attributes["fieldName"].InnerText, x)).ToMap(kvp => kvp.Key, kvp => kvp.Value) ?? new Map<XmlElement>();
    }
  }
}