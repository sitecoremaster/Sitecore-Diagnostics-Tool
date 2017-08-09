namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using JetBrains.Annotations;

  public sealed class Identity
  {
    [PublicAPI]
    public Identity(IdentityType type, string userName, string password)
    {
      Type = type;
      UserName = userName;
      Password = password;
    }

    public IdentityType Type { get; }

    public string UserName { get; }

    public string Password { get; }

    public override string ToString()
    {
      if (Type == IdentityType.SpecificUser)
      {
        return ".\\" + UserName;
      }

      return Type.ToString();
    }
  }
}