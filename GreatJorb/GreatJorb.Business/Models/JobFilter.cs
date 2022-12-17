namespace GreatJorb.Business.Models;

public class JobFilter
{
    public string Query { get; set; } = "";

    public decimal? Salary { get; set; }

    public JobType JobTypeFilter { get; set; } = JobType.Unknown;

    public SalaryType SalaryTypeFilter { get; set; } = SalaryType.Unknown;
    public WorkplaceType WorkplaceTypeFilter { get; set; } = WorkplaceType.Unknown;
    public JobLevel JobLevelFilter { get; set; } = JobLevel.Unknown;

    public JobFilter() { }

    public JobFilter(string query)
    {
        Query = query;
    }

    public bool IsMatch(JobPosting posting)
    {
        if (JobLevelFilter != JobLevel.Unknown && !JobLevelFilter.HasFlag(posting.JobLevel))
            return false;

        if (WorkplaceTypeFilter != WorkplaceType.Unknown && !WorkplaceTypeFilter.HasFlag(posting.WorkplaceType))
            return false;

        if (SalaryTypeFilter != SalaryType.Unknown && !SalaryTypeFilter.HasFlag(posting.SalaryType))
            return false;

        if (JobTypeFilter != JobType.Unknown && !JobTypeFilter.HasFlag(posting.JobType))
            return false;

        if (Salary.HasValue && posting.SalaryMax.GetValueOrDefault() < Salary)
            return false;

        return true;
    }

}
