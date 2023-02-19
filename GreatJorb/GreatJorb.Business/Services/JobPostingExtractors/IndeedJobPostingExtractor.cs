namespace GreatJorb.Business.Services.JobPostingExtractors;

public class IndeedJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => Site.Indeed.ToString();

    public IndeedJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string GetStorageKeyFromUrl(string url)
    {
        return $"Indeed {url.GetQuerystringOrHashValue("vjk")} {url.GetQuerystringOrHashValue("advn")}";
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        var jobTitles = await page.QuerySelectorAllAsync(".jcs-JobTitle");

        List<JobPosting> jobs = new();
        foreach(var title in jobTitles)
        {
            await title.ClickAsync();
            await page.WaitForDOMIdle(cancellationToken);

            jobs.Add(await ExtractJobDetail(page, cancellationToken));
        }

    }

    public async Task<JobPosting> ExtractJobDetail(IPage page, CancellationToken cancellation)
    {
        var jobContainer = await page.WaitForSelectorSafeAsync(".jobsearch-ViewJobLayout-jobDisplay", 
            cancellation,
            retryUntilFound: true);


        var job = new JobPosting();
        if (jobContainer == null)
            return job;

        job.Uri = new Uri(page.Url);
        job.StorageKey = GetStorageKeyFromUrl(page.Url);

        job.Title = await jobContainer
            .QuerySelectorAsync("h2.jobsearch-JobInfoHeader-title")
            .GetInnerText();

        job.Company = await jobContainer
            .QuerySelectorAsync("div[data-company-name='true']")
            .GetInnerText();

        job.DescriptionHtml = await jobContainer
            .QuerySelectorAsync("#jobDescriptionText")
            .GetInnerHTML();

        var dataLines = await jobContainer.ExtractTextFromLeafNodes("div", delimiter: '•');
        job.Location = dataLines[0];
        foreach (var text in dataLines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, text));
        }

        return job;
    }
}
