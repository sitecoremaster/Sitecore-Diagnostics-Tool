namespace Sitecore.DiagnosticsTool.WinApp.ViewModel
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.Reporting;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.WinApp.Command;
  using Sitecore.DiagnosticsTool.WinApp.Model;
  using Sitecore.DiagnosticsTool.WinApp.Resources;

  public class DiagnosticsPageViewModel : WizardPageViewModelBase, ISyncableModel, IExecutableModel
  {
    #region Constructors

    public DiagnosticsPageViewModel(DataSource source)
      : base(source)
    {
    }

    #endregion

    #region Fields

    private ICommand cancelButtonCommand;

    [NotNull]
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    private string reportPath;

    private bool isThreadRunning;

    private int testsNumber;

    private bool isThreadAborted;

    private int currentTest;

    #endregion Fields

    #region Properties

    public int CurrentTest
    {
      get
      {
        return currentTest;
      }
      set
      {
        currentTest = value + 1;
        OnPropertyChanged("CurrentTest");
        OnPropertyChanged("CurrentValue");
        OnPropertyChanged("StatusLabel");
      }
    }

    public int CurrentValue
    {
      get
      {
        if (IsThreadRunning && CurrentTest > 0 || !IsThreadAborted && CurrentTest > 0)
        {
          return CurrentTest * 100 / TestsNumber;
        }

        return 0;
      }
    }

    public bool IsThreadAborted
    {
      get
      {
        return isThreadAborted;
      }
      set
      {
        isThreadAborted = value;
        OnPropertyChanged("IsThreadAborted");
        OnPropertyChanged("StatusLabel");
      }
    }

    public bool IsThreadRunning
    {
      get
      {
        return isThreadRunning;
      }
      set
      {
        isThreadRunning = value;
        OnPropertyChanged("IsThreadRunning");
        OnPropertyChanged("CurrentValue");
        OnPropertyChanged("StatusLabel");
      }
    }

    private string ReportRoot
    {
      get
      {
        var info = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sitecore", "Diagnostics Tool"));
        return !info.Exists ? Directory.CreateDirectory(info.FullName).FullName : info.FullName;
      }
    }

    public int TestsNumber
    {
      get
      {
        return testsNumber;
      }
      set
      {
        testsNumber = value;
        OnPropertyChanged("TestsNumber");
        OnPropertyChanged("CurrentValue");
      }
    }

    public string StatusLabel
    {
      get
      {
        if (CurrentTest == 0)
        {
          return string.Empty;
        }
        if (IsThreadRunning && CurrentTest > 0)
        {
          return string.Format(Strings.DiagnosticsRunning, CurrentTest, TestsNumber);
        }

        return IsThreadAborted ? Strings.DiagnosticsInterrupted : Strings.DiagnosticsCompleted;
      }
    }

    #endregion Properties

    #region Methods

    private string GetReportPath()
    {
      return $"{ReportRoot}\\SitecoreDiagnosticsReport_{DateTime.Now:yymmdd}_{DateTime.Now:hhmm}.html";
    }

    private void RunDiagnostics()
    {
      RefreshData();

      try
      {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
        var system = new SystemContext(assemblyName);
        var packages = Source.Packages
          .Select(package => new SupportPackageDataProvider(package.Path, package.Roles, null))
          .ToArray();

        try
        {
          var resultsFile = TestRunner.RunTests(packages, system, (test, index, count) => OnTestRun(index));

          if (tokenSource.IsCancellationRequested)
          {
            return;
          }

          try
          {
            var path = GetReportPath();
            File.WriteAllText(path, ReportBuilder.GenerateReport(resultsFile));
            reportPath = path;
          }
          catch
          {
            Source.ErrorMessage = Strings.ReportError;
          }
        }
        finally
        {
          foreach (var package in packages)
          {
            try
            {
              package.Dispose();
            }
            catch
            {
            }
          }
        }
      }
      catch
      {
        IsThreadAborted = true;
        Source.ErrorMessage = Strings.DiagnosticsError;
      }
      finally
      {
        IsThreadRunning = false;
        Thread.Sleep(50);
      }
    }

    private void OnTestRun(int index)
    {
      CurrentTest = index;
      if (tokenSource.IsCancellationRequested)
      {
        throw new OperationCanceledException();
      }
    }

    private void RefreshData()
    {
      IsThreadRunning = true;
      IsThreadAborted = false;
      TestsNumber = new TestManager().GetTests().Count();
      CurrentTest = 0;
      reportPath = null;
    }

    #endregion

    #region WizardPageViewModelBase members

    public override string DisplayName => Strings.PageDisplayName_Diagnostics;

    public override string Icon => Strings.Icon_Diagnostics;

    public override string Title => Strings.PageTitle_Diagnostics;

    public override bool IsValid()
    {
      return CurrentTest > 0 && !IsThreadRunning;
    }

    #endregion

    #region Commands

    public ICommand CancelButtonCommand => cancelButtonCommand ?? (cancelButtonCommand = new RelayCommand(CancelButtonClick, CanCancelButtonClick));

    private void CancelButtonClick(object param)
    {
      tokenSource.Cancel();
      IsThreadAborted = true;
      IsThreadRunning = false;
      Source.ErrorMessage = Strings.DiagnosticsCanceled;
    }

    private bool CanCancelButtonClick()
    {
      return IsThreadRunning;
    }

    #endregion

    #region ISyncableModel Members

    public void Sync()
    {
      Source.ReportPath = reportPath;
    }

    #endregion ISyncableModel Members

    #region IExecutableModel Members

    public Task Execute()
    {
      tokenSource = new CancellationTokenSource();
      var token = tokenSource.Token;

      return Task.Factory.StartNew(RunDiagnostics, token).ContinueWith(task => { CommandManager.InvalidateRequerySuggested(); }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    #endregion IExecutableModel Members
  }
}