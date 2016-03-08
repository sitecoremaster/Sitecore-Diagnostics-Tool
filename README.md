# Sitecore Diagnostics Toolset

Sitecore Diagnostics Toolset allows automatically diagnosing Sitecore sites. It does the following:
- Checks the site for consistency of configuration 
- Checks the site for symptoms described in known issues
- Checks the site against recommendations from Sitecore documentation
- Generates HTML report based on the analysis

Sitecore 7.2 and above is the target Sitecore version to support, however it is expected to work with Sitecore 6.3+.

The application is distributed as a Windows Application, and as a Web module to install on the Sitecore site.

### DOWNLOADS & RELEASE NOTES

Available as a [ClickOnce installer](http://dl.sitecore.net/updater/sdt), or portable installations on the [GitHub Releases](https://github.com/Sitecore/Sitecore-Diagnostics-Toolset/releases) page.

### SOURCE CODE & CONTRIBUTION

The source code is not available at the moment. The only contribution we accept now is bug/wish reports.

### PREREQUISITES

* Windows 7, 8.x, 10, 2008 R2, 2012
* .NET Framework 4.0 (4.5 for WebApp version)

### FEEDBACK & BUG REPORTS

For feedback, please use:
a) [GitHub Issues](https://github.com/Sitecore/Sitecore-Diagnostics-Toolset/issues)
b) [Comments on Sitecore Marketplace](https://marketplace.sitecore.net/Modules/Sitecore_Diagnostics_Toolset.aspx)

If reporting an issue, please do not forget to provide:

* the report file (if was successfully generated), or otherwise the log files, located in the `%APPDATA%\Sitecore\Sitecore Diagnostics Toolset\Logs` folder
* an SSPG package of an Instance that you tried to troubleshoot (with extra `C11` and `C12` options checked)

### TROUBLESHOOTING & KNOWN ISSUES

* The Windows version of application requires Administrator permissions.
* If some tests could not be run, review the HTML source of the resulting report - it contains internal log information in the bottom of the document that can point to something useful.
