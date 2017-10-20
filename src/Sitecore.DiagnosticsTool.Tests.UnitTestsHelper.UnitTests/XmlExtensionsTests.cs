namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.UnitTests
{
  using System;
  using System.Linq;
  using System.Xml;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Helpers.XPath;

  [TestClass]
  public class XmlExtensionsTests
  {
    [TestMethod]
    public void CreateTest()
    {
      {
        var doc = new XmlDocument().Create("/test");
        var allNodes = doc.SelectElements("//*");
        Assert.AreEqual(1, allNodes.Length);
        var test = allNodes.OfType<XmlElement>().Single();
        Assert.IsNotNull(test);
        Assert.AreEqual("test", test.Name);
      }

      {
        var doc = new XmlDocument().Create("/test[@hello='/abc/']");
        var allNodes = doc.SelectElements("//*");
        Assert.AreEqual(1, allNodes.Length);
        var test = allNodes.OfType<XmlElement>().Single();
        Assert.IsNotNull(test);
        Assert.AreEqual("test", test.Name);
        Assert.AreEqual("/abc/", test.GetAttribute("hello"));
      }

      {
        var doc = new XmlDocument().Create("/test[@hello='123']");
        var allNodes = doc.SelectElements("//*");
        Assert.AreEqual(1, allNodes.Length);
        var test = allNodes.OfType<XmlElement>().Single();
        Assert.IsNotNull(test);
        Assert.AreEqual("test", test.Name);
        Assert.AreEqual("123", test.GetAttribute("hello"));
      }

      {
        var doc = new XmlDocument().Create("/test[@hello='123']/ololo[@a='abc' and @b='def']");
        var allNodes = doc.SelectElements("//*");
        Assert.AreEqual(2, allNodes.Length);
        var allElements = allNodes.OfType<XmlElement>();
        var test = allElements.First();
        Assert.IsNotNull(test);
        Assert.AreEqual("test", test.Name);
        Assert.AreEqual("123", test.GetAttribute("hello"));

        var ololo = test.GetChildren().Single();
        Assert.AreEqual("ololo", ololo.Name);
        Assert.AreEqual("abc", ololo.GetAttribute("a"));
        Assert.AreEqual("def", ololo.GetAttribute("b"));
      }
    }

    [TestMethod]
    public void GetNodeSegmentsTest()
    {
      GetNodeSegmentsTestSuccess("/n.o-d_e/", "n.o-d_e");
      new Action(() => XPathHelper.GetNodeSegments("//").ToArray().DoNothing())
        .ShouldThrow<NotSupportedException>()
        .WithMessage("Axes are not supported (//)");

      GetNodeSegmentsTestFail("/n.o-d_e]", "open bracket is missing");
      GetNodeSegmentsTestFail("/n.o-d_e'", "open bracket is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[", "attribute name is missing");
      GetNodeSegmentsTestFail("/n.o-d_e['", "attribute name is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a''", "equals sign is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a='", "close quote is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a='[", "close quote is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a='[", "close quote is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a=']", "close quote is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a=''", "close bracket or ' and ' is missing");
      GetNodeSegmentsTestFail("/n.o-d_e[a=''", "close bracket or ' and ' is missing");

      // 1
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/']", "a", "/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[']", "a", "[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']']", "a", "]");

      // 2-1
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='//']", "a", "//");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/[']", "a", "/[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/]']", "a", "/]");

      // 2-2
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[/']", "a", "[/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[[']", "a", "[[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[]']", "a", "[]");

      // 3-1-1
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='///']", "a", "///");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='//[']", "a", "//[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='//]']", "a", "//]");

      // 3-1-2
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/[/']", "a", "/[/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/[[']", "a", "/[[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/[]']", "a", "/[]");

      // 3-1-3
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/]/']", "a", "/]/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/][']", "a", "/][");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='/]]']", "a", "/]]");

      // 3-2-1
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[//']", "a", "[//");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[/[']", "a", "[/[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[/]']", "a", "[/]");

      // 3-2-2
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[[/']", "a", "[[/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[[[']", "a", "[[[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[[]']", "a", "[[]");

      // 3-2-3
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[]/']", "a", "[]/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[][']", "a", "[][");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='[]]']", "a", "[]]");

      // 3-3-1
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']//']", "a", "]//");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']/[']", "a", "]/[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']/]']", "a", "]/]");

      // 3-3-2
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='][/']", "a", "][/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='][[']", "a", "][[");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a='][]']", "a", "][]");

      // 3-3-3
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']]/']", "a", "]]/");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']][']", "a", "]][");
      GetNodeSegmentsTestSuccessAttribute("/n.o-d_e[a=']]]']", "a", "]]]");

      new Action(() => XPathHelper.GetNodeSegments("/node ]").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (open bracket is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/node][").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (open bracket is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/node[ /node[@a='b']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (attribute name is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/node ]/node[@a='b']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (open bracket is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/node][/node[@a='b']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (open bracket is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/ node [ a ] /node[@a=']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (equals sign is missing)");

      new Action(() => XPathHelper.GetNodeSegments("/ node [ a 'a' ] /node[@a=']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (equals sign is missing)");

      // here a attribute value is "]/node[@aa="
      new Action(() => XPathHelper.GetNodeSegments("/node/node[@a=']/node[@aa='b']").ToArray().DoNothing())
        .ShouldThrow<InvalidXPathException>()
        .WithMessage("Invalid xpath (close bracket or ' and ' is missing)");

      {
        var res = XPathHelper.GetNodeSegments("/hello[@html='<a href=\"ftp://usr:pwd@sitecore.net/#abc\">[link]</a>' and  @empty='']").ToArray();
        Assert.AreEqual(1, res.Length);
        var segment = res[0];
        Assert.IsNotNull(segment);
        Assert.AreEqual("hello", segment.Name);
        var html = segment.Attributes.SingleOrDefault(x => x.Key == "html");
        Assert.IsNotNull(html);
        Assert.AreEqual("<a href=\"ftp://usr:pwd@sitecore.net/#abc\">[link]</a>", html.Value);
        var empty = segment.Attributes.SingleOrDefault(x => x.Key == "empty");
        Assert.IsNotNull(empty);
        Assert.AreEqual(string.Empty, empty.Value);
      }

      new Action(() => XPathHelper.GetNodeSegments("/node//node").ToArray().DoNothing())
        .ShouldThrow<NotSupportedException>()
        .WithMessage("Axes are not supported (//)");
    }

    private static void GetNodeSegmentsTestSuccessAttribute(string xpath, string name, string value)
    {
      var res = XPathHelper.GetNodeSegments(xpath).ToArray();
      Assert.AreEqual(1, res.Length);
      var seg = res[0];
      Assert.IsNotNull(seg);
      Assert.AreEqual("n.o-d_e", seg.Name);
      Assert.AreEqual(name, seg.Attributes.Single().Key);
      Assert.AreEqual(value, seg.Attributes.Single().Value);
    }

    private static void GetNodeSegmentsTestFail(string xpath, string message)
    {
      new Action(() => XPathHelper.GetNodeSegments(xpath).ToArray().DoNothing()).ShouldThrow<InvalidXPathException>().WithMessage("Invalid xpath (" + message + ")");
    }

    private void GetNodeSegmentsTestSuccess(string xpath, string name)
    {
      {
        var res = XPathHelper.GetNodeSegments(xpath).ToArray();
        Assert.AreEqual(1, res.Length);
        var seg = res[0];
        Assert.IsNotNull(seg);
        Assert.AreEqual(name, seg.Name);
      }
    }

    [TestMethod]
    public void AddTest()
    {
      {
        var doc1 = new XmlDocument().Create("/root/test1");
        var root = doc1.DocumentElement;
        Assert.IsNotNull(root);
        var doc2 = doc1.Add(root, "/test2");
        Assert.AreEqual(doc1, doc2);
        root = doc1.DocumentElement;
        Assert.IsNotNull(root);
        Assert.AreEqual("root", root.Name);
        Assert.AreEqual(2, root.ChildNodes.Count);
        var test1 = root.ChildNodes[0];
        Assert.IsNotNull(test1);
        Assert.AreEqual("test1", test1.Name);
        var test2 = root.ChildNodes[1];
        Assert.IsNotNull(test2);
        Assert.AreEqual("test2", test2.Name);
      }

      {
        var doc1 = new XmlDocument().Create("/root/test1");
        var root = doc1.DocumentElement;
        Assert.IsNotNull(root);
        var doc2 = doc1.Add(root, "/test2[@hello='123']");
        Assert.AreEqual(doc1, doc2);
        root = doc1.DocumentElement;
        Assert.IsNotNull(root);
        Assert.AreEqual("root", root.Name);
        Assert.AreEqual(2, root.ChildNodes.Count);
        var test1 = root.ChildNodes[0];
        Assert.IsNotNull(test1);
        Assert.AreEqual("test1", test1.Name);
        var test2 = root.ChildNodes[1] as XmlElement;
        Assert.IsNotNull(test2);
        Assert.AreEqual("test2", test2.Name);
        Assert.AreEqual("123", test2.GetAttribute("hello"));
      }
    }
  }
}