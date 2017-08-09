namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using System.Xml;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Xunit;

  public class SchedulingConsistencyTests : SchedulingConsistency
  {
    [Fact]
    public void Passed_MissingElement()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument().Create("/configuration/sitecore/scheduling")
        })
        .Process(this) // missing frequency - that's okay, default value is used               
        .Done();
    }

    [Fact]
    public void Passed_Empty()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument().Create("/configuration/sitecore/scheduling/frequency")
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Warn_Wrong()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument().Create("/configuration/sitecore/scheduling/frequency", "ololo")
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, WrongFrequencyMessage))
        .Done();
    }

    [Fact]
    public void Warn_Stopped()
    {
      var configuration = new XmlDocument().Create("/configuration/sitecore/scheduling/frequency", "00:00:00");
      var frequencyElement = configuration.SelectSingleNode("/configuration/sitecore/scheduling/frequency") as XmlElement;

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = configuration
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetSchedulingStoppedMessage(frequencyElement)))
        .Done();
    }

    [Fact]
    public void Warn_TooLate()
    {
      var configuration = new XmlDocument().Create("/configuration/sitecore/scheduling/frequency", "01:00:00");
      var frequencyElement = configuration.SelectSingleNode("/configuration/sitecore/scheduling/frequency") as XmlElement;

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = configuration
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetShedulingRareMessage(frequencyElement)))
        .Done();
    }
  }
}