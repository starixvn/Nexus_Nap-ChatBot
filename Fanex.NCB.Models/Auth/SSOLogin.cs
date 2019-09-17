namespace Fanex.NCB.Models.Auth
{
    public class SsoLogin
    {
        public string UserId { get; set; }

        public string OnUserId { get; set; }

        public string Time { get; set; }

        public string RedirectUrl { get; set; }
    }
}