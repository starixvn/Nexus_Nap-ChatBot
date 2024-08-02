namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISpecialDeleteService
    {
        Task PullVoidingMessageLicenseeAsync();

        Task PullVoidingMessageInternalDepartmentAsync();
    }
}