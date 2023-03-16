namespace GreatJorb.Business.Models;

public class JobFilter : ILocalStorable
{
    public Guid Id { get; set; }

    public string Query { get; set; } = "";

    public WatchWord[] WatchWords { get; set; } = Array.Empty<WatchWord>();

    public decimal? Salary { get; set; }

    public Site Sites { get; set; } = Site.None;

    public JobType JobTypeFilter { get; set; } = JobType.Unknown;

    public SalaryType SalaryTypeFilter { get; set; } = SalaryType.Unknown;
    public WorkplaceType WorkplaceTypeFilter { get; set; } = WorkplaceType.Unknown;
    public JobLevel JobLevelFilter { get; set; } = JobLevel.Unknown;

    public string StorageKey => Id.ToString();

    public JobFilter() { }

    public JobFilter(string query)
    {
        Query = query;
    }
}

public record JobFilterCacheHeader(Guid Id, DateTime Date) : ILocalStorable
{
    public string StorageKey => Id.ToString();
}

public record JobFilterCacheResult(DateTime Date, JobFilter Filter);