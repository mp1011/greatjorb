namespace GreatJorb.Business.Services.JobPostingExtractors;

public class DiceJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;
    
    public string WebsiteName => Site.Dice.ToString();

    public DiceJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string GetStorageKeyFromUrl(string url)
    {
        url = url.SubstringUpTo('?');
        return "Dice " + url.Split('/').Last();
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var cards = await page.QuerySelectorAllSafeAsync("dhi-search-card", cancellationToken);

        List<string> detailUrls = new();
        foreach(var card in cards)
        {
            var urlElement = card.QuerySelectorAsync("a.card-title-link");
            if (urlElement == null)
                continue;

            var url = await urlElement.GetAttribute("href");

            if (knownJobs.Contains(GetStorageKeyFromUrl(url)))
                continue;

            await page.GoToAsync(url);
            await page.WaitForDOMIdle(cancellationToken);
            return await ExtractJobDetail(page, cancellationToken);
        }

        return null;
    }

    public async Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    public async Task<JobPosting> ExtractJobDetail(IPage page, CancellationToken cancellation)
    {
        var job = new JobPosting();
        job.Uri = new Uri(page.Url);
        job.StorageKey = GetStorageKeyFromUrl(page.Url);

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
