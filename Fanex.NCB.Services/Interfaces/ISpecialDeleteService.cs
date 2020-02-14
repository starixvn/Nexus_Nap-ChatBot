namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISpecialDeleteService
    {
        Task VoidingTicketToLicensee();

        Task VoidingAbnormalBetToInternalDepartment();
    }
}