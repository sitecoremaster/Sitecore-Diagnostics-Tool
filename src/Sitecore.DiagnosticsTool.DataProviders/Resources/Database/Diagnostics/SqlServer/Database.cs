namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SqlClient;
  using System.Globalization;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public class Database
  {
    [NotNull]
    private SqlConnection Connection { get; }

    public Database([NotNull] SqlConnection connection)
    {
      Assert.ArgumentNotNull(connection, nameof(connection));

      Connection = connection;
      Name = connection.Database;

      Tables = new List<Table>();
      StoredProcedures = new List<StoredProcedure>();
      InitializeTables();
      InitializeSoredProcedures();
    }

    public string Name { get; set; }

    public string Key { get; set; }

    public List<Table> Tables { get; set; }

    public List<StoredProcedure> StoredProcedures { get; set; }

    private void InitializeTables()
    {
      var tablesSchema = Connection.GetSchema("Tables");

      foreach (DataRow row in tablesSchema?.Rows)
      {
        if (row == null)
        {
          continue;
        }

        var tableName = row["TABLE_NAME"].ToString();
        var columns = GetColumns(tableName);
        var indexes = GetIndexes(tableName);
        var statistics = GetTableStatistics(tableName);

        var table = new Table
        {
          Name = tableName,
          Columns = columns,
          Indexes = indexes,
          RowCount = (long)statistics["rows"],
          DataSpaceUsed = (double)statistics["size"]
        };

        Tables.Add(table);
      }
    }

    private void InitializeSoredProcedures()
    {
      var storedProceduresDataTable = new DataTable();
      const string Query = @"SELECT A.NAME AS NAME, M.definition as DEFINITION
                            FROM SYS.SQL_MODULES M 
                            INNER JOIN SYS.OBJECTS A ON M.OBJECT_ID = A.OBJECT_ID
                            WHERE A.TYPE_DESC = 'SQL_STORED_PROCEDURE' and Left(NAME, 3) NOT IN ('sp_', 'xp_', 'ms_', 'dt_')
                            ORDER BY TYPE_DESC";

      using (var command = new SqlCommand(Query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(storedProceduresDataTable);
      }

      foreach (DataRow row in storedProceduresDataTable?.Rows)
      {
        if (row == null)
        {
          continue;
        }

        var definition = row["DEFINITION"].ToString();

        var parts = definition.Split(new[] { "\nAS\nBEGIN\n" }, StringSplitOptions.None);
        var storedProcedure = new StoredProcedure
        {
          Name = row["NAME"].ToString()
        };

        try
        {
          storedProcedure.TextHeader = $"{parts[0]}\nAS\n";
          storedProcedure.TextBody = $"BEGIN\n{parts[1]}";
        }
        catch
        {
          storedProcedure.TextBody = definition;
          storedProcedure.TextHeader = string.Empty;
        }

        StoredProcedures.Add(storedProcedure);
      }
    }

    private List<Column> GetColumns(string tableName)
    {
      var query = string.Format(CultureInfo.InvariantCulture, "SELECT TOP 1 * FROM {0}", tableName);
      DataTable schema;

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        using (var reader = command.ExecuteReader())
        {
          schema = reader.GetSchemaTable();
        }
      }

      var primaryKeys = GetConstraintList(tableName, "Primary Key");
      var foreignKeys = GetConstraintList(tableName, "Foreign Key");
      var defaultConstraints = GetDefaultConstraintList(tableName);

      var columns = new List<Column>();
      if (schema == null)
      {
        return columns;
      }

      foreach (DataRow row in schema.Rows)
      {
        if (row == null)
        {
          continue;
        }

        int length;
        var name = row["ColumnName"].ToString();

        var column = new Column
        {
          Name = name,
          DataType = row["DataTypeName"].ToString(),
          Nullable = row["AllowDBNull"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase),
          MaximumLength = int.TryParse(row["ColumnSize"].ToString(), out length) ? length : 0,
          InPrimaryKey = primaryKeys.Contains(name),
          IsForeignKey = foreignKeys.Contains(name),
          DefaultConstraint = defaultConstraints[name]
        };

        columns.Add(column);
      }

      return columns;
    }

    private List<string> GetConstraintList(string tableName, string keyType)
    {
      var constraints = new DataTable();
      var query = $@"SELECT keys.COLUMN_NAME
									FROM information_schema.TABLE_CONSTRAINTS as constraints, information_schema.KEY_COLUMN_USAGE as keys
									WHERE keys.TABLE_NAME = '{tableName}' and constraints.TABLE_NAME = '{tableName}' and CONSTRAINT_TYPE = '{keyType}'";

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(constraints);
      }

      return constraints.Rows?.Cast<DataRow>().Select(x => x[0].ToString()).ToList() ?? new List<string>();
    }

    private Dictionary<string, string> GetDefaultConstraintList(string tableName)
    {
      var defaultConstraints = new DataTable();
      var query = $@"select COLUMN_NAME, COLUMN_DEFAULT
									from INFORMATION_SCHEMA.COLUMNS
									where TABLE_NAME = '{tableName}'";

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(defaultConstraints);
      }

      return defaultConstraints.AsEnumerable().ToDictionary(row => row.Field<string>(0), row => row.Field<string>(1));
    }

    private List<Index> GetIndexes(string tableName)
    {
      var indexes = new List<Index>();
      var indexesDataTable = new DataTable();
      var query = $@"SELECT IndexName = ind.name,
									ColumnName = col.name,
									FileGroup = fil.name,
									ind.type_desc, 
									ind.ignore_dup_key,
									ind.fill_factor,
									ind.is_padded,
									ind.allow_row_locks,
									ind.allow_page_locks,
									ind.filter_definition,
									ic.is_descending_key,
									ic.is_included_column,
									col.is_computed
									FROM 
									sys.filegroups fil,							 
									sys.indexes ind 
									INNER JOIN 
											 sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
									INNER JOIN 
											 sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
									INNER JOIN 
											 sys.tables t ON ind.object_id = t.object_id 
									WHERE 
											 ind.is_primary_key = 0 
											 AND ind.is_unique = 0 
											 AND ind.is_unique_constraint = 0 
											 AND t.is_ms_shipped = 0 
											 AND t.name = '{tableName}'
											 AND ind.data_space_id = fil.data_space_id
									ORDER BY 
											 t.name, ind.name, ind.index_id, ic.index_column_id ";

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(indexesDataTable);
      }

      foreach (DataRow row in indexesDataTable.Rows)
      {
        var indexName = row["IndexName"].ToString();

        var indexedColumn = new IndexedColumn
        {
          Name = row["ColumnName"].ToString(),
          Descending = row["is_descending_key"].ToString().Equals("1", StringComparison.InvariantCultureIgnoreCase),
          IsIncluded = row["is_included_column"].ToString().Equals("1", StringComparison.CurrentCultureIgnoreCase)
        };

        int fillFactor;
        var indexHealth = GetIndexHealth(tableName, indexName);

        var index = new Index
        {
          Name = indexName,
          IndexType = row["type_desc"].ToString(),
          FilterDefinition = row["filter_definition"].ToString(),
          IgnoreDuplicateKeys = row["ignore_dup_key"].ToString().Equals("1", StringComparison.InvariantCultureIgnoreCase),
          DisallowPageLocks = row["allow_page_locks"].ToString().Equals("0", StringComparison.InvariantCultureIgnoreCase),
          DisallowRowLocks = row["allow_row_locks"].ToString().Equals("0", StringComparison.InvariantCultureIgnoreCase),
          FillFactor = int.TryParse(row["allow_row_locks"].ToString(), out fillFactor) ? fillFactor : 0,
          PadIndex = row["allow_row_locks"].ToString().Equals("1", StringComparison.CurrentCultureIgnoreCase),
          FileGroup = row["FileGroup"].ToString(),
          AverageFragmentationInPercent = (double)indexHealth["Fragmentation"],
          PageCount = (long)indexHealth["PageCount"]
        };

        index.IndexedColumns.Add(indexedColumn);
        indexes.Add(index);
      }

      return indexes;
    }

    private Dictionary<string, object> GetIndexHealth(string table, string index)
    {
      var indexHealth = new Dictionary<string, object>();
      var indexHealthDataTable = new DataTable();
      var query = $@"SELECT DISTINCT indexstats.avg_fragmentation_in_percent Fragmentation, indexstats.page_count PageCount
									FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) indexstats 
									INNER JOIN sys.indexes ind  
									ON ind.object_id = indexstats.object_id 
									AND ind.index_id = indexstats.index_id 
									WHERE ind.name = '{index}' and OBJECT_NAME(ind.OBJECT_ID) = '{table}'";

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(indexHealthDataTable);
      }

      double fragmentation;
      long pageCount;

      var row = indexHealthDataTable.Rows[0].IsNotNull("No rows");

      double.TryParse(row["Fragmentation"]?.ToString(), out fragmentation);
      long.TryParse(row["PageCount"]?.ToString(), out pageCount);

      indexHealth.Add("Fragmentation", fragmentation);
      indexHealth.Add("PageCount", pageCount);

      return indexHealth;
    }

    private Dictionary<string, object> GetTableStatistics(string tableName)
    {
      var statistics = new Dictionary<string, object>();

      var statisticsDataTable = new DataTable();
      var query = $@"exec sp_spaceused {tableName}";

      using (var command = new SqlCommand(query, Connection))
      {
        command.CommandTimeout = int.MaxValue;

        var dataAdapter = new SqlDataAdapter(command);
        dataAdapter.Fill(statisticsDataTable);
      }

      long count;
      long.TryParse(statisticsDataTable.Rows[0]["rows"].ToString(), out count);
      statistics.Add("rows", count);

      double size;
      double.TryParse(statisticsDataTable.Rows[0]["data"].ToString().Replace(" KB", string.Empty), out size);
      statistics.Add("size", size);

      return statistics;
    }

    private static IEnumerable<string> GetColumnNames(IDataReader reader)
    {
      var schemaTable = reader.GetSchemaTable();
      var dataRowCollection = schemaTable?.Rows;
      if (dataRowCollection == null)
      {
        yield break;
      }

      foreach (DataRow row in dataRowCollection)
      {
        if (row == null)
        {
          continue;
        }

        yield return (string)row["ColumnName"];
      }
    }

    public Dictionary<string, List<string>> GetTableContent(string tableName)
    {
      var content = new Dictionary<string, List<string>>();

      try
      {
        using (var command = new SqlCommand("select * from " + tableName, Connection))
        {
          command.CommandTimeout = int.MaxValue;

          using (var reader = command.ExecuteReader())
          {
            var columnNames = GetColumnNames(reader).ToList();
            var columnValues = new List<string>();
            content.Add("ColumnNames", columnNames);
            var numFields = columnNames.Count;

            while (reader.Read())
            {
              columnValues = Enumerable.Range(0, numFields)
                .Select(i => reader.GetValue(i).ToString())
                .Select(field => string.Concat("\"", field.Replace("\"", "\"\""), "\""))
                .ToList();
            }

            content.Add("ColumnValues", columnValues);
          }
        }

        return content;
      }
      catch
      {
        return null;
      }
    }
  }
}