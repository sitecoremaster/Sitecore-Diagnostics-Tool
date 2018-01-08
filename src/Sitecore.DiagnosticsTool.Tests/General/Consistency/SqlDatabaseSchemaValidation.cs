namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: TODO (2017-07-13)
  [UsedImplicitly]
  public class SqlDatabaseSchemaValidation : Test
  {
    public override string Name { get; } = "Database schema validation";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var defaultDatabases = data.SitecoreInfo.SitecoreDefaults.SqlDatabases;
      var databases = data.Databases.Sql.All;
      foreach (var database in databases)
      {
        if (database == null)
        {
          continue;
        }

        try
        {
          ProcessDatabase(database, defaultDatabases, data, output);
        }
        catch (ResourceNotAvailableException)
        {
          throw new DatabaseResourceNotAvailableException();
        }
        catch (Exception ex)
        {
          var message = $"Unhandled exception happened during processing {database.Name} database. Find details in log file.";

          output.Debug(ex, message);
          output.CannotRun(message);
        }
      }
    }

    private static void ProcessDatabase([NotNull] ISqlDatabase database, [NotNull] IReadOnlyDictionary<string, IReleaseDefaultSqlDatabase> defaultDatabases, [NotNull] IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(defaultDatabases, nameof(defaultDatabases));
      Assert.ArgumentNotNull(data, nameof(data));

      var databaseName = database.Name;
      if (string.IsNullOrEmpty(databaseName) || !defaultDatabases.ContainsKey(databaseName))
      {
        return;
      }

      var connectionString = database.ConnectionString;
      if (string.IsNullOrEmpty(connectionString))
      {
        return;
      }

      var actualSchema = database.Schema;
      var defaultDatabase = defaultDatabases[databaseName];
      Assert.IsNotNull(defaultDatabase, "defaultDatabase");

      var defaultSchema = defaultDatabase.Schema;
      foreach (var tablePair in defaultSchema.Tables)
      {
        var tableName = tablePair.Key;
        Assert.IsNotNull(tableName, "tableName");

        if (!actualSchema.Tables.ContainsKey(tableName))
        {
          output.Error($"Database schema mismatch: {databaseName}.Tables.dbo.{tableName} - missing");

          continue;
        }

        var actualTable = actualSchema.Tables[tableName];
        Assert.IsNotNull(actualTable, "actualTable");

        var defaultTable = tablePair.Value;
        Assert.IsNotNull(defaultTable, "defaultTable");

        var actualColumns = actualTable.Columns;
        foreach (var defaultColumnPair in defaultTable.Columns)
        {
          var columnName = defaultColumnPair.Key;
          Assert.IsNotNull(columnName, "columnName");

          if (!actualColumns.ContainsKey(columnName))
          {
            output.Error($"Database schema mismatch: {databaseName}.Tables.dbo.{tableName}.Columns.{columnName} - missing");

            continue;
          }

          var defaultColumn = defaultColumnPair.Value;
          Assert.IsNotNull(defaultColumn, "defaultColumn");

          var actualColumn = actualColumns[columnName];
          Assert.IsNotNull(actualColumn, "actualColumn");

          var columnProperties = new[]
          {
            new Property
            {
              Title = "Type",
              Default = defaultColumn.Type,
              Actual = actualColumn.Type,
              IsError = true
            },

            // TODO: enable that when SSPG schema matches SDT/SIS
            //new Property
            //{
            //  Title = "Length",
            //  Default = defaultColumn.Length,
            //  Actual = actualColumn.Length,
            //  IsError = false
            //},
            new Property
            {
              Title = "Nullable",
              Default = defaultColumn.Nullable,
              Actual = actualColumn.Nullable,
              IsError = true
            },
            new Property
            {
              Title = "PrimaryKey",
              Default = defaultColumn.PrimaryKey,
              Actual = actualColumn.PrimaryKey,
              IsError = true
            },
            new Property
            {
              Title = "ForeignKey",
              Default = defaultColumn.ForeignKey,
              Actual = actualColumn.ForeignKey,
              IsError = false
            }

            // TODO: enable that when SSPG schema matches SDT/SIS
            //new Property
            //{
            //  Title = "Constraint",
            //  Default = defaultColumn.Constraint,
            //  Actual = actualColumn.Constraint,
            //  IsError = false
            //},
          };

          ProcessProperties(data, output, columnProperties, "Database schema mismatch: {0}.Tables.dbo.{1}.Columns.{2}", databaseName, tableName, columnName);
        }

        // TODO: remove when SDT can generate accurate indexes schema for local instance
        return;

        var actualIndexes = actualTable.Indexes;
        foreach (var defaultIndexPair in defaultTable.Indexes)
        {
          var indexName = defaultIndexPair.Key;
          Assert.IsNotNull(indexName, "indexName");

          if (!actualIndexes.ContainsKey(indexName))
          {
            var message = $"Database schema mismatch: {databaseName}.Tables.dbo.{tableName}.Indexes.{indexName} - missing";
            output.Error(message);

            continue;
          }

          var actualIndex = actualIndexes[indexName];
          Assert.IsNotNull(actualIndex, "actualIndex");

          var defaultIndex = defaultIndexPair.Value;
          Assert.IsNotNull(defaultIndex, "defaultValue");

          var indexProperties = new[]
          {
            new Property
            {
              Title = "Type",
              Default = defaultIndex.Type,
              Actual = actualIndex.Type,
              IsError = true
            },
            new Property
            {
              Title = "IgnoreDuplicateKeys",
              Default = defaultIndex.IgnoreDuplicateKeys,
              Actual = actualIndex.IgnoreDuplicateKeys,
              IsError = false
            },
            new Property
            {
              Title = "DisallowPageLocks",
              Default = defaultIndex.DisallowPageLocks,
              Actual = actualIndex.DisallowPageLocks,
              IsError = false
            },
            new Property
            {
              Title = "DisallowRowLocks",
              Default = defaultIndex.DisallowRowLocks,
              Actual = actualIndex.DisallowRowLocks,
              IsError = false
            }
          };

          ProcessProperties(data, output, indexProperties, "{0}.Tables.dbo.{1}.Indexes.{2}", databaseName, tableName, indexName);

          var actualIndexColumns = actualIndex.Columns;
          var defaultIndexColumns = defaultIndex.Columns;
          foreach (var defaultIndexColumnPair in defaultIndexColumns)
          {
            var columnName = defaultIndexColumnPair.Key;
            Assert.IsNotNull(columnName, "columnName");

            if (!actualIndexColumns.ContainsKey(columnName))
            {
              output.Error($"Database schema mismatch: {databaseName}.Tables.dbo.{tableName}.Columns.{columnName} - missing");

              continue;
            }

            var actualIndexColumn = actualIndexColumns[columnName];
            Assert.IsNotNull(actualIndexColumn, "actualIndexColumn");

            var defaultIndexColumn = defaultIndexColumnPair.Value;
            Assert.IsNotNull(defaultIndexColumn, "defaultIndexColumn");

            var indexColumnProperties = new[]
            {
              new Property
              {
                Title = "Descending",
                Default = defaultIndexColumn.Descending,
                Actual = actualIndexColumn.Descending,
                IsError = false
              },
              new Property
              {
                Title = "IsComputed",
                Default = defaultIndexColumn.IsComputed,
                Actual = defaultIndexColumn.IsComputed,
                IsError = false
              },
              new Property
              {
                Title = "IsIncluded",
                Default = defaultIndexColumn.IsIncluded,
                Actual = defaultIndexColumn.IsIncluded,
                IsError = false
              }
            };

            ProcessProperties(data, output, indexColumnProperties, "{0}.Tables.dbo.{1}.Indexes.{2}.Columns.{3}", databaseName, tableName, indexName, columnName);
          }

          foreach (var actualIndexColumnPair in actualIndexColumns)
          {
            var columnName = actualIndexColumnPair.Key;
            Assert.IsNotNull(columnName, "columnName");

            if (!actualIndexColumns.ContainsKey(columnName))
            {
              output.Warning($"Database schema mismatch: {databaseName}.Tables.dbo.{tableName}.Columns.{columnName} - unexpected column");
            }
          }
        }
      }

      foreach (var procedure in defaultSchema.StoredProcedures)
      {
        var procedureName = procedure.Key;
        Assert.IsNotNull(procedureName, "procedureName");

        var actualSet = actualSchema.StoredProcedures;
        if (!actualSet.ContainsKey(procedureName))
        {
          var message = $"Database schema mismatch: {databaseName}.Programmability.Stored Procedures.{procedureName} - missing";
          output.Error(message);

          continue;
        }

        var defaultProcedure = procedure.Value;
        Assert.IsNotNull(defaultProcedure, "defaultProcedure");

        var actualProcedure = actualSet[procedureName];
        Assert.IsNotNull(actualProcedure, "actualProcedure");

        var procedureProperties = new[]
        {
          new Property
          {
            Title = "Header",
            Default = defaultProcedure.Header,
            Actual = actualProcedure.Header,
            IsError = true
          },
          new Property
          {
            Title = "Body",
            Default = defaultProcedure.Body,
            Actual = actualProcedure.Body,
            IsError = false
          }
        };

        ProcessProperties(data, output, procedureProperties, "{0}.Programmability.Stored Procedures.{1}", databaseName, procedureName);
      }
    }

    [StringFormatMethod("pathFormat")]
    private static void ProcessProperties([NotNull] IInstanceResourceContext data, ITestOutputContext output, [NotNull] IEnumerable<Property> properties, [NotNull] string pathFormat, params object[] arguments)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(properties, nameof(properties));
      Assert.ArgumentNotNull(pathFormat, nameof(pathFormat));

      // defaults
      arguments = arguments ?? new object[0];

      var path = arguments.Length > 0 ? string.Format(pathFormat, arguments) : pathFormat;
      foreach (var property in properties)
      {
        Assert.IsNotNull(property, "property");

        var actual = property.Actual;
        var expected = property.Default;
        if (actual == null && expected == null)
        {
          continue;
        }

        if (actual != null && actual.ToString().Equals((expected ?? string.Empty).ToString(), StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        var message = $"Database schema mismatch: {path}.{property.Title}. Expected: {expected ?? "<null>"}. Actual: {actual ?? "<null>"}.";
        if (property.IsError)
        {
          output.Error(message);
        }
        else
        {
          output.Warning(message);
        }
      }
    }

    private class Property
    {
      [NotNull]
      public string Title { get; set; }

      public object Default { get; set; }

      public object Actual { get; set; }

      public bool IsError { get; set; }
    }
  }
}