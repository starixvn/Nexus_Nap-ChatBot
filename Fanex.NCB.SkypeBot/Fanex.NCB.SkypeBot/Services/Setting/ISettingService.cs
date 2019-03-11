using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Services.Setting
{
    public interface ISettingService
    {
        Task<List<RecipientType>> GetRecipientTypesAsync();
    }
}
