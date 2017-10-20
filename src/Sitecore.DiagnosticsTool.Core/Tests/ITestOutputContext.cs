namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Output;

  /// <summary>
  ///   The context interface for accessing resources and returning messages.
  /// </summary>
  public interface ITestOutputContext
  {
    /// <summary>
    ///   The method for saving information about error in customer's solution.
    /// </summary>
    /// <param name="message">
    ///   The short message about error in customer's solution.
    /// </param>
    /// <param name="url">
    ///   The link URL to the article that describes the situation.
    /// </param>
    /// <param name="detailed">
    ///   The detailed message about an issue applied to customer's solution.
    /// </param>
    void Error([NotNull] ShortMessage message, [CanBeNull] Uri url = null, [CanBeNull] DetailedMessage detailed = null);

    /// <summary>
    ///   The method for saving information about trivial error or potential place for improvement in customer's solution.
    /// </summary>
    /// <param name="message">
    ///   The short message about trivial error or potential place for improvement in customer's solution.
    /// </param>
    /// <param name="url">
    ///   The link URL to the article that describes the situation.
    /// </param>
    /// <param name="detailed">
    ///   The detailed message about trivial error or potential place for improvement in customer's solution.
    /// </param>
    void Warning([NotNull] ShortMessage message, [CanBeNull] Uri url = null, [CanBeNull] DetailedMessage detailed = null);

    /// <summary>
    ///   The method for notifying that test is not completed due specific circumstance.
    /// </summary>
    /// <param name="message">
    ///   The short message that describes why test couldn't be run.
    /// </param>
    /// <param name="url">
    ///   The link URL to the article that describes the situation.
    /// </param>
    /// <param name="detailed">
    ///   The detailed message that describes why test couldn't be run.
    /// </param>
    void CannotRun([NotNull] ShortMessage message, [CanBeNull] Uri url = null, [CanBeNull] DetailedMessage detailed = null);

    /// <summary>
    ///   The method for saving additional debug information.
    /// </summary>
    /// <param name="detailed">
    ///   The detailed debug information.
    /// </param>
    void Debug([NotNull] DetailedMessage detailed);

    /// <summary>
    ///   The method for saving additional debug information.
    /// </summary>
    /// <param name="exception">
    ///   The exception was thrown.
    /// </param>
    /// <param name="detailed">
    ///   The detailed debug information.
    /// </param>
    void Debug([NotNull] Exception exception, [NotNull] DetailedMessage detailed);
  }
}