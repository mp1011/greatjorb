namespace GreatJorb.Business.Models;

public class JobFilter
{
    public string Query { get; set; } = "";

    public decimal? Salary { get; set; }

    public Site Sites { get; set; } = Site.None;

    public JobType JobTypeFilter { get; set; } = JobType.Unknown;

    public SalaryType SalaryTypeFilter { get; set; } = SalaryType.Unknown;
    public WorkplaceType WorkplaceTypeFilter { get; set; } = WorkplaceType.Unknown;
    public JobLevel JobLevelFilter { get; set; } = JobLevel.Unknown;

    public JobFilter() { }

    public JobFilter(string query)
    {
        Query = query;
    }



}
