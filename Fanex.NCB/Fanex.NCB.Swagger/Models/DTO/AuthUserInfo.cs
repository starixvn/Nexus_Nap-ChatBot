using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NCB.Swagger.Models.DTO
{
    [Serializable]
    public class AuthUserInfo
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