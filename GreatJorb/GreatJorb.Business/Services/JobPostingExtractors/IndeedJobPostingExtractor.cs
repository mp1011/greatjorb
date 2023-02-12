namespace GreatJorb.Business.Services.JobPostingExtractors;

public class IndeedJobPostingExtractor : IJobPostingExtractor
{
    public string WebsiteName => Site.Indeed.ToString();

    public Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        throw new NotImplementedException();
    }
}
