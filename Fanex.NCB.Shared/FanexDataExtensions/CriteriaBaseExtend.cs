namespace Fanex.NCB.Shared.FanexDataExtensions
{
    using Fanex.Data.Repository;

    public interface ICriteriaExtend : ICriteria
    {
        string ServerIp { get; }
        string DbName { get; }
    }

    public abstract class CriteriaBaseExtend : ICriteriaExtend
    {
        public abstract string ServerIp { get; set; }

        public abstract string DbName { get; set; }

        public abstract string GetSettingKey();

        public abstract bool IsValid();
    }
}