using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NCB.Core.Models.DTO;

namespace Fanex.NCB.Main.Models.DTO
{
    public class ApplicationUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// Ex: Tkey = permission key
        /// TValue = Policy (0= read, 1= host)
        /// </summary>
        public IDictionary<string, int> Permissions { get; set; }
    }
}
