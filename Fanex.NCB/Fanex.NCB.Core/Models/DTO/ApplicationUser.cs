using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fanex.NCB.Main.Models.DTO
{
    public class ApplicationUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public IEnumerable<string> Menu { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
