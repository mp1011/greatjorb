namespace GreatJorb.Business.Models;

public class JobPosting : ILocalStorable
{
    private const int WorkHoursPerYear = 2080;

    public string StorageKey { get; set; }

    public Uri Uri { get; set; } = new Uri("https://none");
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public JobType JobType { get; set; }
    public WorkplaceType WorkplaceType { get; set; }
    public SalaryType SalaryType { get; set; }
    public JobLevel JobLevel { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? DescriptionHtml { get; set; }

    public decimal? SalaryMinAsAnnual => AsAnnual(SalaryMin);
    public decimal? SalaryMaxAsAnnual => AsAnnual(SalaryMax);

    private decimal? AsAnnual(decimal? amount)
    {
        if (amount == null)
            return null;

        if (SalaryType == SalaryType.Hourly || amount <= 1000)
            amount *= WorkHoursPerYear;

        return amount;
    }

    public string[] MiscProperties { get; set; } = Array.Empty<string>();

    public JobPosting(string url, string storageKey)
    {
        Uri = new Uri(url);
        StorageKey = storageKey;
    }

    public JobPosting() : this("https://none", "https://none")
    {
    }
}
 
