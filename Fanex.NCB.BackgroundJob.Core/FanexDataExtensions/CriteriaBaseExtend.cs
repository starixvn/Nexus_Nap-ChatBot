namespace Fanex.NCB.AutoJobs.Core.FanexDataExtensions
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public interface ICriteriaExtend : ICriteria
    {
        string ServerIp { get; set; }

        string DbName { get; set; }
    }

    public abstract class CriteriaBaseExtend : ICriteriaExtend
    {
        [SpParam(Ignored = true)]
        public abstract string ServerIp { get; set; }

        [SpParam(Ignored = true)]
        public abstract string DbName { get; set; }

        public abstract string GetSettingKey();

        public abstract bool IsValid();

        public bool IsInValid() => !IsValid();
    }
}