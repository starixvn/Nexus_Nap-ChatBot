namespace Fanex.NCB.Shared.FanexDataExtensions.ObjectDb
{
    using System.Data.SqlClient;
    using Fanex.Data;
    using Fanex.Data.SqlServer;

    public class SqlObjectDbExtend : ObjectDb
    {
        public SqlObjectDbExtend(string newServerIp, string dbName, string key, int? commandTimeout = null)
            : base(key, commandTimeout)
        {
            var currentConnectionString = DbSetting.ConnectionString;
            var sqlConnection = new SqlConnectionStringBuilder(currentConnectionString);

            sqlConnection.DataSource = newServerIp;

            var newSettings = new DbSetting(key,
                !string.IsNullOrWhiteSpace(dbName) ? dbName : DbSetting.DbName,
                sqlConnection.ConnectionString,
                DbSetting.SpName,
                commandTimeout,
                DbSetting.ColumnMaps,
                DbSetting.FirmParameters);
            DbSetting = newSettings;
            Runner = new SqlStoredProcedureRunner(DbSetting.DbName, DbSetting.ConnectionString);
        }
    }
}