namespace Fanex.NCB.AutoJobs.Core.Repository
{
    using Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using System.Collections;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }

    public class BaseRepository : IBaseRepository
    {
        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command)
        {
            using var db = ObjectDbFactory.CreateInstance(command.GetSettingKey());
            await db.ExecuteNonQueryAsync(command);
            var commandResult = GetCommandResult(db.GetOutputParams());

            return commandResult;
        }

        private static CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errCode"];
            var errMsg = hash["errMsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}