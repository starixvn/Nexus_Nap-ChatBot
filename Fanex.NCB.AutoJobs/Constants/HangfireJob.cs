namespace Fanex.NCB.AutoJobs.Constants
{
    public static class HangfireJob
    {
        public const string SendNotification = "SendNotification";

        public const string SyncNapUserControlData = "SyncNapUserControlData";

        public const string SyncOICData = "SyncOICData";

        public const string VoidingTicket = "PullTicketFromSpecialDeleteAndScanNoti";

        public const string VoidingAbnormalBets = "VoidingAbnormalBets";

        public const string MonitorSkypebot = "MonitorSkypebot";

        public const string OneMinuteJobs = "OneMinuteJobs";
    }
}