namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class SchedulingConsistency : Test
  {
    protected const string WrongFrequencyMessage = "The <frequency> element in the <scheduling> element of the web.config file contains value which cannot be recognized as TimeStamp.";

    public override string Name { get; } = "Scheduling consistency";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var configuration = data.SitecoreInfo.Configuration;
      var frequencyElement = configuration.SelectSingleNode("/configuration/sitecore/scheduling/frequency") as XmlElement;
      var text = frequencyElement?.InnerText;
      if (string.IsNullOrEmpty(text))
      {
        // default 00:01:00 frequency is used
        return;
      }

      TimeSpan frequency;
      if (!TimeSpan.TryParse(text, out frequency))
      {
        output.Warning(WrongFrequencyMessage);

        // default 00:01:00 frequency is used
        return;
      }

      if (frequency.Ticks == 0)
      {
        output.Warning(GetSchedulingStoppedMessage(frequencyElement));
      }
      else if (frequency.Hours >= 1)
      {
        output.Warning(GetShedulingRareMessage(frequencyElement));
      }
    }

    [NotNull]
    protected static string GetSchedulingStoppedMessage([NotNull] XmlElement frequencyElement)
    {
      Assert.ArgumentNotNull(frequencyElement, nameof(frequencyElement));

      return $"All the scheduled tasks are stopped by {frequencyElement.OuterXml} element in the web.config file";
    }

    [NotNull]
    protected static string GetShedulingRareMessage([NotNull] XmlElement frequencyElement)
    {
      Assert.ArgumentNotNull(frequencyElement, nameof(frequencyElement));

      return $"All the scheduled agents perform too rare (once per {frequencyElement.InnerText}) due to {frequencyElement.OuterXml} element in the web.config file";
    }
  }
}