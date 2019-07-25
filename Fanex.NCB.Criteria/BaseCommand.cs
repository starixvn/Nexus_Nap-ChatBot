namespace Fanex.NCB.Criteria
{
    using Fanex.Data.Repository;

    public abstract class BaseCommand : NonQueryCommand
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }
    }
}