namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using System;
  using System.Linq;
  using System.Xml;

  using FluentAssertions;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  using Xunit;

  public class LogicalDatabaseDefinitionTests
  {
    [Fact]
    public void Parse_NoID()
    {
      // arrange
      var xml = new XmlDocument();
      xml.LoadXml($"<database/>");

      // act and assert
      new Action(() => LogicalDatabaseDefinition.Parse(xml.DocumentElement, new SitecoreVersion(8, 2, 0, 000000)))
        .ShouldThrow<ArgumentException>()
        .WithMessage("The id attribute is missing or whitespace.\r\nXml: <database />\r\nParameter name: definition");
    }

    [Fact]
    public void Parse_OnlyID()
    {
      // arrange
      var xml = new XmlDocument();
      xml.LoadXml($"<database id='db123'/>");

      // act
      var db = LogicalDatabaseDefinition.Parse(xml.DocumentElement, new SitecoreVersion(8, 2, 0, 000000));

      // assert
      Assert.Equal("db123", db.Name);
      Assert.Equal(6, db.Caches.Count);
      Assert.True(db.Caches.All(x => x.Value.Size == null));
    }

    [Fact]
    public void Parse_OnlyDataCache_ValidValue()
    {
      // arrange
      var xml = new XmlDocument();
      xml.LoadXml($"<database id='db123'><cacheSizes><data>4MB</data></cacheSizes></database>");

      // act
      var db = LogicalDatabaseDefinition.Parse(xml.DocumentElement, new SitecoreVersion(8, 2, 0, 000000));

      // assert
      Assert.Equal("db123", db.Name);
      Assert.Equal(6, db.Caches.Count);
      Assert.True(db.Caches["items"].Size == null);
      Assert.True(db.Caches["path"].Size == null);
      Assert.True(db.Caches["prefetch"].Size == null);
      Assert.True(db.Caches["standardValues"].Size == null);
      Assert.True(db.Caches["itempaths"].Size == null);

      var data = db.Caches["data"].Size;
      Assert.NotNull(data);
      Assert.Equal(4, data.Value.MB);
      Assert.Equal("4MB", data.Text);
    }

    [Fact]
    public void Parse_OnlyDataCache_InvalidValue()
    {
      // arrange
      var xml = new XmlDocument();
      xml.LoadXml($"<database id='db123'><cacheSizes><data>4mb</data></cacheSizes></database>");

      // act
      var db = LogicalDatabaseDefinition.Parse(xml.DocumentElement, new SitecoreVersion(8, 2, 0, 000000));

      // assert
      Assert.Equal("db123", db.Name);
      Assert.Equal(6, db.Caches.Count);
      Assert.True(db.Caches["items"].Size == null);
      Assert.True(db.Caches["path"].Size == null);
      Assert.True(db.Caches["prefetch"].Size == null);
      Assert.True(db.Caches["standardValues"].Size == null);
      Assert.True(db.Caches["itempaths"].Size == null);

      var data = db.Caches["data"].Size;
      Assert.NotNull(data);
      Assert.Equal(-1, data.Value.Bytes);
      Assert.Equal("4mb", data.Text);
    }
  }
}