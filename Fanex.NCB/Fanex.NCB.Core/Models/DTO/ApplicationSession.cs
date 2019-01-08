using System.Collections.Generic;
using Fanex.NCB.Common.DTO;

namespace Fanex.NCB.Core.Models.DTO
{
    public class ApplicationSession
    {
        public IList<Menu> MenuList { get; set; }
        public IList<AuthorizedFunction> AuthorizedFunction { get; set; }
    }
}
