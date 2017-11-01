namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using FluentAssertions;

  using Sitecore.DiagnosticsTool.Core.Categories;

  using Xunit;

  public class CategoryTests
  {
    [Fact]
    public void AllCategoriesTest()
    {
      var categories = Category.GetAll();
      categories.Should().NotBeNull();
      var i = 0;
      foreach (var category in categories)
      {
        category.Should().NotBeNull("categories[" + i + "]");
        category.Name.Should().NotBeNullOrEmpty("categories[" + i++ + "].Name");
      }
    }
  }
}