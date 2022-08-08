namespace Fanex.NCB.Shared.FanexDataExtensions
{
    using System.Data.SqlClient;
    using Fanex.Data;
    using Fanex.Data.SqlServer;

    public static class DynamicDbExtensions
    {
        private static IDbSetting GetCurrentDbSetting(string settingKey)
        {
            var dbSettingProvider = DbSettingProviderManager.GetCurrentDbSettingProvider();
            var dbSetting = dbSettingProvider.GetSetting(settingKey);
            return dbSetting;
        }

        public static (IStoredProcedureRunner, DbCommandDefinition) GetRunnerCommand(ICriteriaExtend criteria)
        {
            var dbSetting = GetCurrentDbSetting(criteria.GetSettingKey());
            var currentConnectionString = dbSetting.ConnectionString;
            var sqlConnection = new SqlConnectionStringBuilder(currentConnectionString);
            sqlConnection.DataSource = criteria.ServerIp;

            IStoredProcedureRunner runner = new SqlStoredProcedureRunner(dbSetting.DbName, sqlConnection.ConnectionString);
            DbCommandDefinition command = runner.CreateDbCommandDefinition(dbSetting.SpName, 120, false);
            return (runner, command);
        }
    }
}