namespace Sitecore.DiagnosticsTool.Core.Resources.Logging
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;

  public class ExceptionInfo
  {
    [NotNull]
    public static readonly ExceptionInfo Empty = new ExceptionInfo(TypeRef.Empty, string.Empty, string.Empty, null);

    [UsedImplicitly]
    public ExceptionInfo InnerException { get; }

    [UsedImplicitly]
    [NotNull]
    public string Message { get; }

    [UsedImplicitly]
    [NotNull]
    public string StackTrace { get; }

    [NotNull]
    [UsedImplicitly]
    public TypeRef Type { get; }

    public ExceptionInfo([NotNull] TypeRef type, [NotNull] string message, [NotNull] string stackTrace, ExceptionInfo innerException)
    {
      Assert.ArgumentNotNull(type, nameof(type));
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(stackTrace, nameof(stackTrace));

      Assert.ArgumentCondition(type != TypeRef.Empty || message == string.Empty, "message", "The message must not be specified when type is empty");
      Assert.ArgumentCondition(type != TypeRef.Empty || stackTrace == string.Empty, "stackTrace", "The stackTrace must not be specified when type is empty");

      Type = type;
      Message = message;
      StackTrace = stackTrace;
      InnerException = innerException;
    }

    public static bool operator ==(ExceptionInfo a, object b)
    {
      return Equals(a, b);
    }

    public static bool operator !=(ExceptionInfo a, object b)
    {
      return !Equals(a, b);
    }

    public static bool operator ==(object a, ExceptionInfo b)
    {
      return Equals(a, b);
    }

    public static bool operator !=(object a, ExceptionInfo b)
    {
      return !Equals(a, b);
    }

    public static bool operator ==(ExceptionInfo a, ExceptionInfo b)
    {
      return Equals(a, b);
    }

    public static bool operator !=(ExceptionInfo a, ExceptionInfo b)
    {
      return !Equals(a, b);
    }

    public static bool Equals(ExceptionInfo a, object b)
    {
      return Equals(a, b as ExceptionInfo);
    }

    public static bool Equals(object a, ExceptionInfo b)
    {
      return Equals(a as ExceptionInfo, b);
    }

    public static bool Equals(ExceptionInfo a, ExceptionInfo b)
    {
      var objA = a as object;
      var objB = b as object;

      if (objA == null && objB == null)
      {
        return true;
      }

      if (objA == null || objB == null)
      {
        return false;
      }

      if (!TypeRef.Equals(a.Type, b.Type))
      {
        return false;
      }

      if (!string.Equals(a.Message, b.Message, StringComparison.Ordinal))
      {
        return false;
      }

      if (!string.Equals(a.StackTrace, b.StackTrace, StringComparison.Ordinal))
      {
        return false;
      }

      return Equals(a.InnerException, b.InnerException);
    }

    public static bool IsNullOrEmpty(ExceptionInfo exception)
    {
      if (exception as object == null)
      {
        return true;
      }

      return TypeRef.IsNullOrEmpty(exception.Type);
    }

    public override bool Equals(object obj)
    {
      return TypeRef.Equals(this, obj as TypeRef);
    }

    public bool Equals(TypeRef obj)
    {
      return TypeRef.Equals(this, obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Type.GetHashCode();

        hashCode = (hashCode * 397) ^ Message.GetHashCode();
        hashCode = (hashCode * 397) ^ StackTrace.GetHashCode();
        hashCode = (hashCode * 397) ^ (InnerException != null ? InnerException.GetHashCode() : 0);

        return hashCode;
      }
    }

    public override string ToString()
    {
      if (TypeRef.IsNullOrEmpty(Type))
      {
        return string.Empty;
      }

      var output = Type.TypeName + ": " + Message;
      var innerException = InnerException;
      if (innerException == null)
      {
        return output + Environment.NewLine + StackTrace;
      }

      var strArray = new[]
      {
        output,
        " ---> ",
        innerException.ToString(),
        Environment.NewLine,
        "   ",
        "--- End of inner exception stack trace ---"
      };

      output = string.Concat(strArray);

      return output + Environment.NewLine + StackTrace;
    }
  }
}