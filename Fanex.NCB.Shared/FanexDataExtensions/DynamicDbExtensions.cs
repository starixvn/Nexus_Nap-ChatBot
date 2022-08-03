namespace Fanex.NCB.Shared.FanexDataExtensions
{
    public static class DynamicDbExtensions
    {
        private const int Command_Timeout = 120;
        private static readonly bool Ignore_Redundant_Parameters = false;

        //public static async Task<IEnumerable<T>> GetDataAsync<T>(ICriteriaExtend criteria)
        //{
        //    var newDbCommand = CreateNewDbcommand(criteria);
        //    var sqlRunner = newDbCommand.Item1;
        //    var dbCommand = newDbCommand.Item2;
        //    return await sqlRunner.QueryAsync<T>(dbCommand);
        //}

        //private static IObjectDb GetObjectDb(string settingKey, int? commandTimeout)
        //{
        //    CreateObjectDbFunc GetCreateObjectDbFunc(string key)
        //    {
        //        var dbSettingProvider = DbSettingProviderManager.GetCurrentDbSettingProvider();
        //        var dbSetting = dbSettingProvider.GetSetting(key);
        //        var connectionAlias = dbSetting.DbName;

        //        return RegisteredProviders[providerName];
        //    }
        //}
    }
}