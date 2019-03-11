using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.BaseStorage
{
    public interface ISettingStorage
    {
        Task<List<RecipientType>> GetRecipientTypesAsync();
    }
}
