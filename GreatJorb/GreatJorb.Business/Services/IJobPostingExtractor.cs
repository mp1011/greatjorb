namespace GreatJorb.Business.Services;

public interface IJobPostingExtractor
{
    string WebsiteName { get; }

    string GetStorageKeyFromUrl(string url);

    Task<JobPosting?> ExtractNextJob(
        IPage page,         
        HashSet<string> knownJobs,
        CancellationToken cancellationToken);

    Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken);
}
