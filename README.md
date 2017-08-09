# Sitecore Diagnostics Tool 1.3

Sitecore Diagnostics Tool 1.3 (SDT) allows automatically diagnosing Sitecore solutions. It does the following:

- Checks all the solution sites for consistency of configuration 
- Checks all the solution sites for symptoms described in known issues
- Checks all the solution sites against recommendations from Sitecore documentation
- Generates HTML report based on the analysis

Sitecore 8.0 and above is the target Sitecore version to support, however it is expected to work with Sitecore 6.3+.

The application is distributed as a Windows Application that can be run only on a developer machine to analyse [SSPG package](https://marketplace.sitecore.net/Modules/Sitecore_Support_Package_Generator.aspx).

### DOWNLOADS & RELEASE NOTES

Available only as a [ClickOnce installer](http://dl.sitecore.net/updater/qa/sdt). 

There is a command-line version that you need to compile yourself - in source code it's SDT project.

### SOURCE CODE & CONTRIBUTION

The SDT 1.3 is open source and source code is available according to MIT license.

### PREREQUISITES

* Windows 7, 8.x, 10, 2008 R2, 2012
* .NET Framework 4.5 

### FEEDBACK & BUG REPORTS

For feedback, please use:  
a) [GitHub Issues](https://github.com/Sitecore/Sitecore-Diagnostics-Tool/issues)  
b) [Comments on Sitecore Marketplace](https://marketplace.sitecore.net/Modules/Sitecore_Diagnostics_Tool.aspx)

If reporting an issue, please do not forget to provide:

* the report file (if was successfully generated), or otherwise the log files, located in the `%APPDATA%\Sitecore\Sitecore Diagnostics Tool\Logs` folder
* all [SSPG packages](https://marketplace.sitecore.net/Modules/Sitecore_Support_Package_Generator.aspx) of a solution that you tried to troubleshoot
