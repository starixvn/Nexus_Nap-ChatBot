namespace Fanex.NCB.AutoJobs.Models;

public class JobSetting
{
    public string JobName { get; set; }

    public bool IsEnabled { get; set; }

    public bool HasCheckUM { get; set; }

    public int TimeInterval { get; set; }
}