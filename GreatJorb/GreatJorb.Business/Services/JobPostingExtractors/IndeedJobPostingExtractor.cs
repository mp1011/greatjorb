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
        var jobTitles = await page.QuerySelectorAllAsync(".jcs-JobTitle");

        List<JobPosting> jobs = new();
        foreach(var title in jobTitles)
        {
            var dataJk = await title.GetAttribute("data-jk");
            var dataEmpn = await title.GetAttribute("data-empn");
            var jobKey = $"Indeed {dataJk} {dataEmpn}";

            if (knownJobs.Contains(jobKey))
                continue;

            await title.ClickAsync();
            await page.WaitForDOMIdle(cancellationToken);

            return await ExtractJobDetail(page, cancellationToken);
        }

        return null;
    }

    public async Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken)
    {
        var pager = await page.QuerySelectorAsync("nav[role='navigation']");
        if (pager == null)
            return false;

        var pagerElements = await pager.QuerySelectorAllAsync("div");

        bool foundSelected = false;

        foreach (var element in pagerElements)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!foundSelected)
            {
                var button = await element.QuerySelectorAsync("button");
                foundSelected = (await button.GetAttribute("data-testid")) == "pagination-page-current";
            }
            else
            {
                await element.ClickAsync();
                await page.WaitForDOMIdle(cancellationToken);
                return true;
            }
        }

        return false;
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

        job.DescriptionHtml = await GetDescriptionElement(page, cancellation)
            .GetInnerHTML();

        var dataLines = await jobContainer.ExtractTextFromLeafNodes("div", delimiter: '•');
        job.Location = dataLines[0];
        foreach (var text in dataLines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, text));
        }

        return job;
    }

    public async Task<IElementHandle?> GetDescriptionElement(IPage page, CancellationToken cancellation)
    {
        var jobContainer = await page.WaitForSelectorSafeAsync(".jobsearch-ViewJobLayout-jobDisplay",
            cancellation,
            retryUntilFound: true);

        if (jobContainer == null)
            return null;

        return await jobContainer
                .QuerySelectorAsync("#jobDescriptionText");
    }
}
