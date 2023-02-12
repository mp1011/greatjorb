namespace GreatJorb.Business.Services.JobPostingExtractors;

public class MonsterJobPostingExtractor : IJobPostingExtractor
{
    public string WebsiteName => Site.Monster.ToString();

    public Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        throw new NotImplementedException();
    }
}
