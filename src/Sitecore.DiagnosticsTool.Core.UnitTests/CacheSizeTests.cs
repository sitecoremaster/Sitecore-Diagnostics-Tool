namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using Ploeh.AutoFixture.Xunit2;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  using Xunit;

  public class CacheSizeTests
  {
    private ISitecoreVersion Sc825 { get; } = new SitecoreVersion(8, 2, 5, 0);

    [Theory]
    [AutoData]
    public void Parse_Text(uint size)
    {
      // arrange
      var text = $"{size}";

      // act
      var sut = CacheSize.Parse(text, Sc825);

      // assert
      Assert.Equal(text, sut.Text);
    }

    [Theory]
    [AutoData]
    public void Parse_Values_Bytes_Valid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}", Sc825);

      // assert
      Assert.Equal(size, sut.Value.Bytes);
    }

    [Theory]
    [AutoData]
    public void Parse_Values_Bytes_Invalid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}b", Sc825);

      // assert
      Assert.Equal(-1, sut.Value.Bytes); // kind weird, but that's what it is
    }

    [Theory]
    [AutoData]
    public void Parse_Values_KB_Valid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}KB", Sc825);

      // assert
      Assert.Equal(size, sut.Value.KB);
    }

    [Theory]
    [AutoData]
    public void Parse_Values_KB_Invalid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}kb", Sc825);

      // assert
      Assert.Equal(0, sut.Value.KB); // kind weird, but that's what it is
    }

    [Theory]
    [AutoData]
    public void Parse_Values_MB_Valid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}MB", Sc825);

      // assert
      Assert.Equal(size, sut.Value.MB);
    }

    [Theory]
    [AutoData]
    public void Parse_Values_MB_Invalid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}mb", Sc825);

      // assert
      Assert.Equal(0, sut.Value.MB); // kind weird, but that's what it is
    }

    [Theory]
    [AutoData]
    public void Parse_Values_GB_Valid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}GB", Sc825);

      // assert
      Assert.Equal(size, sut.Value.GB);
    }

    [Theory]
    [AutoData]
    public void Parse_Values_GB_Invalid_825(uint size)
    {
      // act
      var sut = CacheSize.Parse($"{size}gb", Sc825);

      // assert
      Assert.Equal(0, sut.Value.GB); // kind weird, but that's what it is
    }
  }
}