namespace Sitecore.DiagnosticsTool.Commands.Abstract
{
  using Sitecore.Diagnostics.FileSystem;

  internal interface IGenericCommand
  {
    string[] Options { get; }

    IFileSystem FileSystem { get; }

    void Execute();
  }
}