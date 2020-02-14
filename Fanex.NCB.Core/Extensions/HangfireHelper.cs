namespace Fanex.NCB.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NCB.Core.Models;

    public static class HangfireHelper
    {
        public static HangfireJobSetting GetJobConfig(IEnumerable<HangfireJobSetting> jobConfigs, string jobName)
        {
            return jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsEnabledJob(IEnumerable<HangfireJobSetting> jobConfigs, string jobName)
        {
            var settings = GetJobConfig(jobConfigs, jobName);

            return settings != null && settings.IsEnabled;
        }
    }
}