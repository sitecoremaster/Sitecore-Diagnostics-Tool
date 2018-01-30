namespace Sitecore.DiagnosticsTool.Commands.Handlers
{
  using Fclp;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.DiagnosticsTool.Commands.Contracts;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;

  internal class DecompressCommandHandler : ICompressCommand
  {
    public string[] Options { get; }

    public IFileSystem FileSystem { get; }

    public DecompressCommandHandler(ICompressCommand args)
    {
      FileSystem = args.FileSystem ?? Program.FileSystem;
      Options = args.Options;

      Assert.ArgumentNotNull(FileSystem);
      Assert.ArgumentNotNull(Options);
    }

    public void Execute()
    {
      var parser = new FluentCommandLineParser();
      IDirectory mega = null;
      parser.Setup<string>('i', "inputDir")
        .WithDescription("Path to the extracted mega SSPG package folder")
        .Callback(x => mega = FileSystem.ParseDirectory(x));

      var result = parser.Parse(Options);
      if (result.HelpCalled)
      {
        return;
      }

      Assert.IsTrue(mega.Exists, $"The folder does not exist: {mega}");
      PackageHelper.DecompressMegaPackage(mega, mega);
    }
  }
}