namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Reporting;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  using Xunit;

  public class ReportBuilderTests
  {
    [Fact]
    public void Test()
    {
      var test = new TestMetadata("MyTest");

      var simple = "There is an issue";
      var link = new Uri("http://localhost");

      var results = new TestResults();
      results.Add(new TestOutput(TestResultState.Error, simple, link, new DetailedMessage(new Container(new Text("Detailed issue variant 1"), new BulletedList("list item 1", "list item 2"))), "instance1"));
      results.Add(new TestOutput(TestResultState.Error, simple, link, new DetailedMessage(new Container(new Text("Detailed issue variant 1"), new BulletedList("list item 1", "list item 2"))), "instance2"));
      results.Add(new TestOutput(TestResultState.Error, simple, link, new DetailedMessage(new Container(new Text("Detailed issue variant 2"), new BulletedList("list item 3"))), "instance3"));
      
      var file = new ResultsFile
      {
        Solution = new ITestReport[]
        {
          new TestReport(test, results),
        }
      };

      var errors = ReportBuilder.GetMessagesText(ReportBuilder.GetErrorMessages(file), "errors").Replace("\r", "").Replace("\n", "").Replace("  ", " ").Replace("  ", " ");      
      AssertEqual(("" +
        " <div class='alert alert-danger alert-dismissible fade show' role='alert'> " +
        "   <button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button> " +
        "   <h4 class='alert-heading'> " +
        "     <a href='#E1'>E1</a>. <span class='test-name'>MyTest</span>" +
        "   </h4>" +
        "   <div class='short-message'>" +
        "     There is an issue" +
        "   </div>" +
        "   <a href='#' data-toggle='modal' data-target='#E1-details'>See&nbsp;full&nbsp;details.</a> " +
        "   <div id='E1-details' class='modal fade' tabindex='-1' role='dialog' aria-labelledby='myLargeModalLabel' aria-hidden='true'> " +
        "     <div class='modal-dialog modal-lg'> " +
        "       <div class='modal-content'> " +
        "         <div class='modal-header'> " +
        "           <h5 class='modal-title'>E1. MyTest</h5>   " +
        "           <button type='button' class='close' data-dismiss='modal' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button>  " +
        "         </div>  " +
        "" +
        "         <div class='modal-body'>" +
        "" +
        "           <div class='detailed-message'>" +
        "             <div class='applies-to'>instance1</div>" +
        "             Detailed issue variant 1" +
        "             <ul>" +
        "               <li>list item 1</li>" +
        "               <li>list item 2</li>" +
        "             </ul>" +
        "             <div>" +
        "               Get more information in <a href='http://localhost/'>this document</a>." +
        "             </div>" +
        "           </div>" +
        "           <hr />" +
        "           <div class='detailed-message'>" +
        "             <div class='applies-to'>instance2</div>" +
        "             Detailed issue variant 1" +
        "             <ul>" +
        "               <li>list item 1</li>" +
        "               <li>list item 2</li>" +
        "             </ul>" +
        "             <div>" +
        "               Get more information in <a href='http://localhost/'>this document</a>." +
        "             </div>" +
        "           </div>" +
        "           <hr />" +
        "           <div class='detailed-message'>" +
        "             <div class='applies-to'>instance3</div>" +
        "             Detailed issue variant 2" +
        "             <ul>" +
        "               <li>list item 3</li>" +
        "             </ul>" +
        "             <div>" +
        "               Get more information in <a href='http://localhost/'>this document</a>." +
        "             </div>" +
        "           </div>  " +
        "" +
        "         </div>  " +
        "       </div> " +
        "     </div> " +
        "   </div> " +
        " </div>"), errors);
    }

    private void AssertEqual(string expected, string actual)
    {
      Assert.Equal(
        expected.Replace(">", "> ").Replace("<", " <").ReplaceWithSpaces("  ").Trim("\r\n\t ".ToCharArray()), 
        actual.Replace(">", "> ").Replace("<", " <").ReplaceWithSpaces("  ").Trim("\r\n\t ".ToCharArray()));
    }

    public class TestMetadata : ITestMetadata
    {
      public string Name { get; }

      public TestMetadata(string name)
      {
        Name = name;
      }

      public IEnumerable<Category> Categories { get; } = new Category[0];
    }
  }

  internal static class Extensions
  {
    public static string ReplaceWithSpaces(this string that, string value)
    {
      while (that.IndexOf(value, StringComparison.Ordinal) >= 0)
      {
        that = that.Replace(value, "  ");
      }

      return that;
    }
  }
}