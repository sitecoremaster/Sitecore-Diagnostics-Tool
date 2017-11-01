namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using Sitecore.DiagnosticsTool.Core.Output;

  using Xunit;

  public class MessageTests
  {
    [Fact]
    public void Test()
    {
      /*
       *|| L1
       *|| * L2s1
       *||   ```
       *||   somecode
       *||   ```
       *|| * L2s1
       *||   * L3s1
       *||     L3s1
       *||   * L3table
       *||     * Row 1
       *||       * Header1 = Value1
       *||       * Header2 = Value2
       *||     * Row 2
       *||       * Header2 = Value2
       *||       * Header3 = Value3
       *||   * L3s1
       *||     L3s1
       */

      var message = new ShortMessage(
        new Text("L1"),
        new BulletedList(
          new Container(
            new Text("L2s1a"),
            new CodeBlock("somecode"),
            new Text("multi\r\nline\r\ntext")
          ),
          new Container(
            new Text("L2s1b"),
            new BulletedList(
              new Container(
                new Text("L3s1a"),
                new Paragraph("L3s1b")
              ),
              new Container(
                new Text("L3table"),
                new Table(
                  new TableRow
                  {
                    {"Header1", "H1-1"},
                    {"Header3", "H3-1"},
                    {"Header4", "H4-1"},
                  },
                  new TableRow
                  {
                    {"Header2", "H2-2"},
                    {"Header1", "H1-2"},
                    {"Header3", "H3-2"},
                  },
                  new TableRow
                  {
                    {"Header2", "H2-2"},
                  }
                )
              ),
              new Container(
                new Text("L3s1c"),
                new Paragraph("L3s1d")
              )
            )
          )
        )
      );

      Assert.Equal($"L1\r\n" +
        $"* L2s1a\r\n" +
        $"  ```\r\n" +
        $"  somecode\r\n" +
        $"  ```\r\n" +
        $"  multi\r\n" +
        $"  line\r\n" +
        $"  text\r\n" +
        $"* L2s1b\r\n" +
        $"  * L3s1a\r\n" +
        $"    L3s1b\r\n" +
        $"  * L3table\r\n" +
        $"    * H1-1\r\n" +
        $"      * Header3 = H3-1\r\n" +
        $"      * Header4 = H4-1\r\n" +
        $"    * H1-2\r\n" +
        $"      * Header3 = H3-2\r\n" +
        $"      * Header2 = H2-2\r\n" +
        $"    * Row 3\r\n" +
        $"      * Header2 = H2-2\r\n" +
        $"  * L3s1c\r\n" +
        $"    L3s1d",
        message.ToString());
    }
  }
}