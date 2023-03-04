namespace GreatJorb.Business.Services;

public interface IJobPostingExtractor
{
    string WebsiteName { get; }

    string GetStorageKeyFromUrl(string url);

    Task<JobPosting?> ExtractNextJob(
        IPage page,         
        HashSet<string> knownJobs,
        CancellationToken cancellationToken);

    Task<IElementHandle?> GetDescriptionElement(IPage page, CancellationToken cancellation);

    Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken);
}
