namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Helpers.XPath
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public static class XPathHelper
  {
    [NotNull]
    public static IEnumerable<XPathSegment> GetNodeSegments([NotNull] string xpath)
    {
      Assert.ArgumentNotNull(xpath, nameof(xpath));

      if (xpath.StartsWith("//"))
      {
        throw new NotSupportedException("Axes are not supported (//)");
      }

      var pos = 0;
      var x = xpath.Trim(" /".ToCharArray()) + "/";

      while (pos < x.Length)
      {
        // skip spaces
        while (IsChar(pos, x, ' '))
        {
          pos++;
        }

        if (IsChar(pos, x, '*'))
        {
          throw new NotSupportedException("Asterisk is not supported");
        }

        if (!IsName(pos, x, false))
        {
          throw new InvalidXPathException("node name is missing", x, pos);
        }

        // node name e.g. "node"
        var node = pos;
        while (IsName(pos, x, false))
        {
          pos++;
        }

        // skip spaces e.g. "node  "
        while (IsChar(pos, x, ' '))
        {
          pos++;
        }

        // no attributes e.g. "node   /"
        var nodeName = x.Substring(node, pos - node);
        if (IsChar(pos, x, '/'))
        {
          pos++;

          if (IsChar(pos, x, '/'))
          {
            throw new NotSupportedException("Axes are not supported (//)");
          }

          yield return new XPathSegment(nodeName);
        }
        else
        {
          if (NotChar(pos, x, '['))
          {
            throw new InvalidXPathException("open bracket is missing", x, pos);
          }

          // attributes started  e.g. "node ["
          pos++;

          var attributes = new List<KeyValuePair<string, string>>();
          while (pos < x.Length)
          {
            // skip spaces  e.g. "node [  "
            while (IsChar(pos, x, ' '))
            {
              pos++;
            }

            if (!IsName(pos, x, true))
            {
              throw new InvalidXPathException("attribute name is missing", x, pos);
            }

            // attribute name started  e.g. "node [ @attr"
            var attribute = pos;
            while (IsName(pos, x, true))
            {
              pos++;
            }

            // skip spaces  e.g. "node [ @attr ="
            while (IsChar(pos, x, ' '))
            {
              pos++;
            }

            if (NotChar(pos, x, '='))
            {
              throw new InvalidXPathException("equals sign is missing", x, pos);
            }

            var attributeName = x.Substring(attribute, pos - attribute);

            // equals starts attribute value
            pos++;

            // skip spaces  e.g. "node [ @attr = "
            while (IsChar(pos, x, ' '))
            {
              pos++;
            }

            if (NotChar(pos, x, '\''))
            {
              throw new InvalidXPathException("open quote is missing", x, pos);
            }

            // value started e.g. "node [ @attr = '"
            pos++;
            var value = pos;

            while (NotChar(pos, x, '\''))
            {
              pos++;
            }

            if (pos >= x.Length)
            {
              throw new InvalidXPathException("close quote is missing", x, pos);
            }

            // value ended
            var attributeValue = x.Substring(value, pos - value);
            attributes.Add(new KeyValuePair<string, string>(attributeName.TrimStart('@'), attributeValue));

            pos++;

            // skip spaces  e.g. "node [ @attr = '123' "
            while (IsChar(pos, x, ' '))
            {
              pos++;
            }

            if (IsChar(pos, x, ']'))
            {
              pos++;
              break;
            }

            if (pos + 4 >= x.Length || x.Substring(pos, 4) != "and ")
            {
              throw new InvalidXPathException("close bracket or ' and ' is missing", x, pos);
            }

            pos += 4;
          }

          if (NotChar(pos, x, '/'))
          {
            throw new InvalidXPathException("slash is missing", x, pos);
          }

          pos++;

          if (IsChar(pos, x, '/'))
          {
            throw new NotSupportedException("Axes are not supported (//)");
          }

          yield return new XPathSegment(nodeName, attributes);
        }
      }
    }

    private static bool IsName(int pos, string x, bool isAttribute)
    {
      return pos < x.Length && x[pos] >= 'a' && x[pos] <= 'z' || x[pos] >= 'A' && x[pos] <= 'Z' || x[pos] == '_' || x[pos] == '.' || x[pos] == '-' || x[pos] >= '0' && x[pos] <= '9' || isAttribute && x[pos] == '@';
    }

    private static bool IsChar(int pos, string x, params char[] cs)
    {
      return x != null && cs != null && pos < x.Length && cs.Any(c => x[pos] == c);
    }

    private static bool NotChar(int pos, string x, params char[] cs)
    {
      return x != null && cs != null && pos < x.Length && cs.All(c => x[pos] != c);
    }
  }
}