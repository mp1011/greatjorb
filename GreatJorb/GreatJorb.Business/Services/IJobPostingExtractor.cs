namespace GreatJorb.Business.Services;

public interface IJobPostingExtractor
{
    string WebsiteName { get; }

    Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber = 0, int? PageSize = null);
}
