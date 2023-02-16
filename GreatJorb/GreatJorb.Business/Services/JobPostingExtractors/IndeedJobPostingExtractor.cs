namespace GreatJorb.Business.Services.JobPostingExtractors;

public class IndeedJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => Site.Indeed.ToString();

    public IndeedJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        var jobTitles = await page.QuerySelectorAllAsync(".jcs-JobTitle");

        List<JobPosting> jobs = new();
        foreach(var title in jobTitles)
        {
            await title.ClickAsync();
            await page.WaitForDOMIdle(cancellationToken);

            jobs.Add(await ExtractJobDetail(page, cancellationToken));
        }

        return jobs.ToArray();
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
        job.StorageKey = page.Url;

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
