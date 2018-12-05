using System;
using System.Collections.Generic;
using System.Text;

namespace Fanex.NCB.Core.Models.DTO
{
    public class ApplicationSession
    {
        public IList<Menu> MenuList { get; set; }
        public IList<AuthorizedFunction> AuthorizedFunction { get; set; }
    }
}
