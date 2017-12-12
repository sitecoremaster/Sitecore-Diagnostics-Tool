namespace Sitecore.DiagnosticsTool.Core.UnitTests.Extensions
{
  using System.Xml;

  using Sitecore.DiagnosticsTool.Core.Extensions;

  using Xunit;

  public class XmlExtensionsTests
  {
    [Fact]
    public void ToString_Default_Simple()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  />");

      Assert.Equal("<doc a=\"3\" />", xml.DocumentElement.ToString(XmlPrintMode.Default));
    }

    [Fact]
    public void ToString_HeaderOnly_Simple()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  />");

      Assert.Equal("<doc a=\"3\" />", xml.DocumentElement.ToString(XmlPrintMode.HeaderOnly));
    }

    [Fact]
    public void ToString_WithoutChildren_Simple()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  />");
      
      Assert.Equal("<doc a=\"3\" />", xml.DocumentElement.ToString(XmlPrintMode.WithoutChildren));
    }

    [Fact]
    public void ToString_Default_Complex()
    {
      var xml = new XmlDocument().Parse("<doc a='3' ><child /></doc>");

      Assert.Equal("<doc a=\"3\"><child /></doc>", xml.DocumentElement.ToString(XmlPrintMode.Default));
    }
    
    [Fact]
    public void ToString_WithoutChildren_Complex()
    {
      var xml = new XmlDocument().Parse("<doc a='3' ><child /></doc>");

      Assert.Equal("<doc a=\"3\" />", xml.DocumentElement.ToString(XmlPrintMode.WithoutChildren));
    }

    [Fact]
    public void ToString_HeaderOnly_Complex()
    {
      var xml = new XmlDocument().Parse("<doc a='3' ><child /></doc>");

      Assert.Equal("<doc a=\"3\">", xml.DocumentElement.ToString(XmlPrintMode.HeaderOnly));
    }

    [Fact]
    public void ToString_Default_Namespace()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  xmlns:test='http://a/'><child /></doc>");

      Assert.Equal("<doc a=\"3\" ... ><child /></doc>", xml.DocumentElement.ToString(XmlPrintMode.Default));
    }

    [Fact]
    public void ToString_HeaderOnly_Namespace()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  xmlns:test='http://a/'><child /></doc>");

      Assert.Equal("<doc a=\"3\" ... >", xml.DocumentElement.ToString(XmlPrintMode.HeaderOnly));
    }

    [Fact]
    public void ToString_WithoutChildren_Namespace()
    {
      var xml = new XmlDocument().Parse("<doc a='3'  xmlns:test='http://a/'><child /></doc>");

      Assert.Equal("<doc a=\"3\" ... />", xml.DocumentElement.ToString(XmlPrintMode.WithoutChildren));
    }
  }
}
