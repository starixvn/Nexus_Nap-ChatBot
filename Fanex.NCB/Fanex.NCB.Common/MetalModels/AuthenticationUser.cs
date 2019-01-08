using System;

namespace Fanex.NCB.Common.MetalModels
{
    [Serializable]
    public class AuthenticationUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}