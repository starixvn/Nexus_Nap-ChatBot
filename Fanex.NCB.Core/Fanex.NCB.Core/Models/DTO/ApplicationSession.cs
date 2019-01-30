using Fanex.NCB.Common.DTO;
using System.Collections.Generic;

namespace Fanex.NCB.Core.Models.DTO
{
    public class ApplicationSession
    {
        public IList<Menu> MenuList { get; set; }
        public IList<AuthorizedFunction> AuthorizedFunction { get; set; }
    }
}
