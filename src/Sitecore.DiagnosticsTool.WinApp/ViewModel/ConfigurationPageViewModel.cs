namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;

  public class ConfigurationPageViewModel : WizardPageViewModelBase, ISyncableModel
  {
    #region Constructors

    public ConfigurationPageViewModel(DataSource source) : base(source)
    {
    }

    #endregion

    #region Fields

    private List<CategorySourceModel> categories;

    #endregion

    #region Properties

    public override string DisplayName => Strings.PageDisplayName_Configuration;
    public override string Icon => Strings.Icon_Configuration;
    public override string Title => Strings.PageTitle_Configuration;

    public List<CategorySourceModel> Categories
    {
      get
      {
        if (categories != null)
        {
          return categories;
        }
        categories = new List<CategorySourceModel>();

        foreach (var category in Category.GetAll())
        {
          var categoryModel = new CategorySourceModel
          {
            Category = category
          };
          if (category == Category.Ecm || category == Category.Wffm)
          {
            categoryModel.IsSelected = false;
          }
          else
          {
            categoryModel.IsSelected = true;
          }
          categories.Add(categoryModel);
        }

        return categories;
      }
    }

    #endregion

    #region Methods

    public override bool IsValid()
    {
      return Categories != null && Categories.Any(c => c.IsSelected);
    }

    public void Sync()
    {
      Source.Categories = Categories.Where(category => category.IsSelected).Select(c => c.Category).ToList();
    }

    #endregion

    #region Nested class CategorySourceModel

    public class CategorySourceModel
    {
      public Category Category { get; set; }
      public bool IsSelected { get; set; }
    }

    #endregion
  }
}