namespace GreatJorb.Business.Services.JobPostingExtractors;

public class GoogleJobsExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public GoogleJobsExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string WebsiteName => Site.GoogleJobs.GetDisplayName();

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var jobHeaders = await page.QuerySelectorAllAsync("li.iFjolb");

        foreach (var jobHeader in jobHeaders)
        {
            var jobId = await GetJobId(page, jobHeader, cancellationToken);
            if (!knownJobs.Contains(jobId))
            {
                return await ExtractJob(jobHeader, page, cancellationToken);
            }
        }

        return null;
    }
    public async Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    private async Task<string> GetJobId(IPage page, IElementHandle element, CancellationToken cancellationToken)
    {
        await element.ClickAsync();
        return GetStorageKeyFromUrl(page.Url);       
    }

    private async Task<JobPosting> ExtractJob(IElementHandle element, IPage page, CancellationToken cancellationToken)
    {
        var lines = await element
            .ExtractTextFromLeafNodes("div,span");
       
        var jobPosting =  new JobPosting
        {
            Title = lines[0],
            Company = lines[1],
            Location = lines[2],
        };

        foreach(var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(jobPosting, line));
        }

        await element.ClickAsync();
        await Task.Delay(200);

        var viewportSize = await page.GetViewportSize();

        var jobDescriptionArea = await page
                .GetElementByPoint(0.7, 0.7)
                .GetAncestor(page, async p =>
                {
                    var bounds = await p.BoundingBoxAsync();
                    return (double)bounds.Height > viewportSize.Height * 0.5;
                }, cancellationToken);

        if (jobDescriptionArea != null)
        {
            jobPosting.DescriptionHtml = await jobDescriptionArea
                .GetInnerHTML();
        }

        jobPosting.Uri = new Uri(page.Url);
        jobPosting.StorageKey = GetStorageKeyFromUrl(page.Url);

        return jobPosting;
    }

    public string GetStorageKeyFromUrl(string url)
    {
        var docid = url.GetQuerystringOrHashValue("htidocid");
        return "Google " + docid;
    }
}
