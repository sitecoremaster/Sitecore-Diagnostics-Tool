namespace Sitecore.DiagnosticsTool.Core.Categories
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  /// <summary>
  ///   Represents a category of tests.
  /// </summary>
  public sealed class Category
  {
    // must be placed before all the rest of static members
    [NotNull]
    private static readonly object SyncRoot = new object();

    // must be placed before all the rest of static members
    [NotNull]
    private static readonly IList<Category> Categories = new List<Category>();

    private Category([NotNull] string name)
    {
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      Name = name;

      lock (SyncRoot)
      {
        Categories.Add(this);
      }
    }

    /// <summary>
    ///   The display name of the category.
    /// </summary>
    [NotNull]
    public string Name { get; }

    /// <summary>
    ///   Gets all registered categories.
    /// </summary>
    /// <returns>All registered categories.</returns>
    [NotNull]
    public static IList<Category> GetAll()
    {
      lock (SyncRoot)
      {
        return Categories;
      }
    }

    #region Categories

    /// <summary>
    ///   The category represents tests that valid for all kinds of Sitecore instances and environments.
    /// </summary>
    [NotNull]
    public static readonly Category General = new Category("General");

    /// <summary>
    ///   The category represents tests that check presence of known security vulnerabilities.
    /// </summary>
    [NotNull]
    public static readonly Category Security = new Category("Security Vulnerabilities");

    /// <summary>
    ///   The category represents tests that designed to improve performance and scalability.
    /// </summary>
    [NotNull]
    public static readonly Category Performance = new Category("Performance and Scalability");

    /// <summary>
    ///   The category represents tests that validate search and indexing features of Sitecore.
    /// </summary>
    [NotNull]
    public static readonly Category SearchIndexing = new Category("Search and Indexing");

    /// <summary>
    ///   The category represents tests that check for Analytics and xDB-related issues.
    /// </summary>
    [NotNull]
    public static readonly Category Analytics = new Category("Analytics/xDB");

    /// <summary>
    ///   The category represents tests that check for Email Experience Manager related issues.
    /// </summary>
    [NotNull]
    public static readonly Category Ecm = new Category("Email Experience Manager");

    /// <summary>
    ///   The category represents tests that check for Web Forms for Marketers related issues.
    /// </summary>
    [NotNull]
    public static readonly Category Wffm = new Category("Web Forms for Marketers");

    /// <summary>
    ///   The category represents tests that intended to check server in production environment.
    /// </summary>
    [NotNull]
    public static readonly Category Production = new Category("Production");

    #endregion
  }
}