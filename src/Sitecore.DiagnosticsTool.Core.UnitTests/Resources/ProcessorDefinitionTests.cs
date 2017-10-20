namespace Sitecore.DiagnosticsTool.Core.UnitTests.Resources
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Xunit;

  public class ProcessorDefinitionTests
  {
    [Fact]
    public void ProcessorDefinition_Static_Equals_null_null()
    {
      var result = ProcessorDefinition.Equals(null, null);

      Assert.Equal(true, result);
    }

    [Fact]
    public void ProcessorDefinition_Static_Equals_null_notnull()
    {
      var result = ProcessorDefinition.Equals(null, new ProcessorDefinition(TypeRef.Empty, null, false, null));

      Assert.Equal(false, result);
    }

    [Fact]
    public void ProcessorDefinition_Static_Equals_notnull_null()
    {
      var result = ProcessorDefinition.Equals(new ProcessorDefinition(TypeRef.Empty, null, false, null), null);

      Assert.Equal(false, result);
    }

    [Fact]
    public void ProcessorDefinition_Static_Equals_same_same()
    {
      var definition = new ProcessorDefinition(TypeRef.Empty, null, false, null);

      var result = ProcessorDefinition.Equals(definition, definition);

      Assert.Equal(true, result);
    }

    [Fact]
    public void ProcessorDefinition_Static_Equals_different_method()
    {
      var a = new ProcessorDefinition(TypeRef.Empty, "A", false, null);
      var b = new ProcessorDefinition(TypeRef.Empty, "B", false, null);

      var result = ProcessorDefinition.Equals(a, b);

      Assert.Equal(false, result);
    }

    [Fact]
    public void ProcessorDefinition_Static_Equals_different_type()
    {
      // ReSharper disable AssignNullToNotNullAttribute
      var a = new ProcessorDefinition(TypeRef.Parse("A"), null, false, null);
      var b = new ProcessorDefinition(TypeRef.Parse("B"), null, false, null);
      // ReSharper enable AssignNullToNotNullAttribute

      var result = ProcessorDefinition.Equals(a, b);

      Assert.Equal(false, result);
    }
  }
}