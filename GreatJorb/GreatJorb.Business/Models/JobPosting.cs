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
    public int? SalaryMin { get; set; }
    public int? SalaryMax { get; set; }
    public string? DescriptionHtml { get; set; }

    public string[] MiscProperties { get; set; } = Array.Empty<string>();
}
 
