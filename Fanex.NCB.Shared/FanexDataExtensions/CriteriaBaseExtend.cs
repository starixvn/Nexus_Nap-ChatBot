namespace Fanex.NCB.Shared.FanexDataExtensions
{
    using Fanex.Data.Repository;

    public interface ICriteriaExtend : ICriteria
    {
        string ServerIp { get; }
        public string Alias { get; }
    }

    public abstract class CriteriaBaseExtend : ICriteriaExtend
    {
        public abstract string ServerIp { get; set; }

        public abstract string Alias { get; set; }

        public abstract string GetSettingKey();

        public abstract bool IsValid();
    }
}