namespace GreatJorb.Business.Services.JobPostingExtractors;

public class SimplyHiredJobPostingExtractor : IJobPostingExtractor
{
    public string WebsiteName => Site.SimplyHired.GetDisplayName();

    public Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        throw new NotImplementedException();
    }
}
