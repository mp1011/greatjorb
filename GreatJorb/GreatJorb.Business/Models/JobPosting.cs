namespace GreatJorb.Business.Models;

public class JobPosting
{ 
    public string? Url { get; set; }
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

    public string[] MiscProperties { get; set; } = Array.Empty<string>();
}
 
