namespace Sitecore.DiagnosticsTool.Core.Categories
{
  using System;

  /// <summary>
  ///   Represents a Sitecore instance server role.
  /// </summary>
  [Flags]
  public enum ServerRole
  {
    /// <summary>
    ///   The server role represents tests that check Content-Management features in the Sitecore instance.
    /// </summary>
    ContentManagement = 1 << 2,

    /// <summary>
    ///   The server role represents tests that check Content-Delivery features in the Sitecore instance.
    /// </summary>
    ContentDelivery = 1 << 3,

    /// <summary>
    ///   Responsible for indexing of Sitecore content databases.
    /// </summary>
    ContentIndexing = 1 << 4,

    /// <summary>
    ///   The server role represents tests that check Content-Delivery features in the Sitecore instance.
    /// </summary>
    Publishing = 1 << 5,

    /// <summary>
    ///   The server role represents tests that check xDB Processing feature in the Sitecore instance.
    /// </summary>
    Processing = 1 << 6,

    /// <summary>
    ///   The server role represents tests that check xDB Reporting feature in the Sitecore instance.
    /// </summary>
    Reporting = 1 << 7,

    /// <summary>
    ///   Responsible for the email dispatch process. This role is only relevant when using EXM module.
    /// </summary>
    EmailDispatching = 1 << 8,
  }
}