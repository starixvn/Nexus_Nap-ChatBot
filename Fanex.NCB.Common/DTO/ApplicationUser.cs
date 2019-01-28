using System;
using System.Collections.Generic;

namespace Fanex.NCB.Common.DTO
{
    [Serializable]
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
