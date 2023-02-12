namespace GreatJorb.Business.Services.JobPostingExtractors;

public class DiceJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;
    
    public string WebsiteName => Site.Dice.ToString();

    public DiceJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        var cards = await page.QuerySelectorAllSafeAsync("dhi-search-card", cancellationToken);

        List<string> detailUrls = new();
        foreach(var card in cards)
        {
            var urlElement = card.QuerySelectorAsync("a.card-title-link");
            if (urlElement != null)
                detailUrls.Add(await urlElement.GetAttribute("href"));
        }

        List<JobPosting> postings = new();

        foreach(var detailUrl in detailUrls)
        {
            await page.GoToAsync(detailUrl);
            await page.WaitForDOMIdle(cancellationToken);
            postings.Add(await ExtractJobDetail(page, cancellationToken));
        }

        return postings.ToArray();
    }

    public async Task<JobPosting> ExtractJobDetail(IPage page, CancellationToken cancellation)
    {
        var job = new JobPosting();
        job.Uri = new Uri(page.Url);
        job.StorageKey = job.Uri.Host + job.Uri.LocalPath;

        var mainElement = await page.QuerySelectorAsync("main");

        if(mainElement != null)
        {
            return await ExtractJobDetail_Type1(page, job, cancellation);
        }

        mainElement = await page.QuerySelectorAsync(".container.job-details");
        if(mainElement != null)
        {
            return await ExtractJobDetail_Type2(page, job, cancellation);
        }

        return job;
    }

    private async Task<JobPosting> ExtractJobDetail_Type1(IPage page, JobPosting job, CancellationToken cancellation)
    {
        job.Title = await page
           .QuerySelectorAsync("h1[data-cy='jobTitle']")
           .GetInnerText();

        job.DescriptionHtml = await page
            .QuerySelectorAsync("article")
            .GetInnerHTML();

        var mainElement = await page.QuerySelectorAsync("main");

        var lines = await mainElement
            .QuerySelectorAllAsync("p,li")
            .GetInnerTextAsync();

        foreach (var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, line));
        }

        job.Location = await page
            .QuerySelectorAsync("li[data-cy='companyLocation']")
            .GetInnerText();

        job.Company = await page
            .QuerySelectorAsync("span[data-cy='companyNameNoLink']")
            .GetInnerText();

        return job;
    }

    private async Task<JobPosting> ExtractJobDetail_Type2(IPage page, JobPosting job, CancellationToken cancellation)
    {
        job.Title = await page
           .QuerySelectorAsync("h1.jobTitle")
           .GetInnerText();

        var mainElement = await page.QuerySelectorAsync(".container.job-details");
        job.DescriptionHtml = await mainElement.GetInnerHTML();

        var lines = await mainElement
           .QuerySelectorAllAsync("p,li")
           .GetInnerTextAsync();

        foreach (var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, line));
        }

        job.Location = await page
            .QuerySelectorAsync("#labellocation")
            .GetInnerText();

        job.Company = await page
            .QuerySelectorAsync(".brcs-title")
            .GetInnerText();

        return job;
    }
}
