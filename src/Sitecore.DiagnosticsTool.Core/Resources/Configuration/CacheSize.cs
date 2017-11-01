namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;

  public class CacheSize
  {
    public Size Value { get; }

    [CanBeNull]
    public string Text { get; }

    public CacheSize(Size value, [CanBeNull] string text)
    {
      Value = value;
      Text = text;
    }

    public override string ToString()
    {
      return Value.ToString();
    }

    [NotNull]
    public static CacheSize Parse([NotNull] string value, ISitecoreVersion version)
    {
      var bytes = GetSizeInBytes(value, version);
      var size = new CacheSize(Size.FromBytes(bytes), value);

      return size;
    }

    private static long GetSizeInBytes([CanBeNull] string value, [NotNull] ISitecoreVersion version)
    {
      if (version.MajorMinorInt == 826) // TODO: update when merged to next major release
      {
        throw new NotImplementedException();
      }

      return ParseSizeString(value);
    }

    // extracted from 8.2.0, Sitecore.MainUtil
    private static long GetLong(object obj, long defaultValue)
    {
      if (obj != null)
      {
        try
        {
          return Convert.ToInt64(obj);
        }
        catch (Exception)
        {
          // silent
        }
      }

      return defaultValue;
    }

    // extracted from 8.2.0, Sitecore.StringUtil
    private static long ParseSizeString(string value)
    {
      if (value != null && value.Length > 0)
      {
        value = value.Replace(".", "");
        value = value.Replace(" ", "");

        int length = value.Length;

        if (length > 0)
        {
          int factor = 1;

          // check for KB, MB, GB
          if (value[length - 1] == 'B')
          {
            switch (value[length - 2])
            {
              case 'K':
                factor = 1024;
                break;
              case 'M':
                factor = 1024 * 1024;
                break;
              case 'G':
                factor = 1024 * 1024 * 1024;
                break;
            }

            value = value.Substring(0, length - 2);
          }

          long size = GetLong(value, -1);

          if (size >= 0)
          {
            return size * factor;
          }
        }
      }

      return -1;
    }
  }
}