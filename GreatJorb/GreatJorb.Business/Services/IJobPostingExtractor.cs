namespace GreatJorb.Business.Services;

public interface IJobPostingExtractor
{
    string WebsiteName { get; }

    Task<JobPosting[]> ExtractJobsFromPage(IPage page, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null);
}
