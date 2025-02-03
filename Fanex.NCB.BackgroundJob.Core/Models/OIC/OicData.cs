namespace Fanex.NCB.AutoJobs.Core.Models.OIC;

using System.Collections.Generic;
using IntegrationMethod;

public class OicData
{
    public IEnumerable<Licensee> Licensees { get; set; }

    public IEnumerable<LicenseeProduct> LicenseeProducts { get; set; }

    public IEnumerable<LicenseeStatus> LicenseeStatuses { get; set; }

    public IEnumerable<LicenseeType> LicenseeTypes { get; set; }

    public IEnumerable<Product> Products { get; set; }

    public IEnumerable<Platform> Platforms { get; set; }

    public IEnumerable<IntegrationMethodInfo> IntegrationMethod { get; set; }

    public IEnumerable<LicenseeIntegrationMethod> LicenseeIntegrationMethod { get; set; }
}