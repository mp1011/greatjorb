namespace GreatJorb.Business.Services.JobPostingExtractors;

public class MonsterJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => Site.Monster.ToString();

    public MonsterJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }


    public string GetStorageKeyFromUrl(string url)
    {
        url = url.SubstringUpTo('?');
        return url.Split('/').Last();
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var cards = await page.QuerySelectorAllAsync("div[class*='JobCardWrap']");

        foreach(var card in cards)
        {
            var url = await card.QuerySelectorAsync("a").GetAttribute("href");
            if (knownJobs.Contains(GetStorageKeyFromUrl(url)))
                continue;

            url = $"https:{url}";
            await page.GoToAsync(url);
            return await ExtractJob(page, url, cancellationToken);
        }

        return null;
    }

    public async Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        var cards = await page.QuerySelectorAllAsync("div[class*='JobCardWrap']");
        var lastCard = cards.Last();

        await lastCard.FocusAsync();

        await page.EvaluateFunctionAsync("e => window.scroll(0, window.scrollY+8)");

        await Task.Delay(100);
        await page.WaitForDOMIdle(cancellationToken);

        var newCards = await page.QuerySelectorAllAsync("div[class*='JobCardWrap']");
        return newCards.Length > cards.Length;
    }

    public async Task<JobPosting> ExtractJob(IPage page, string url, CancellationToken cancellationToken)
    {
        var jobContainer = await page.QuerySelectorAsync("div[class*='jobview-container']");

        var job = new JobPosting();
        job.Uri = new Uri(url);
        job.StorageKey = GetStorageKeyFromUrl(page.Url); 

        job.Title = await jobContainer
            .QuerySelectorAsync(".JobViewTitle")
            .GetInnerText();

        job.Company = await jobContainer
            .QuerySelectorAsync("h2[class*='JobViewHeaderCompany']")
            .GetInnerText();

        var lines = await jobContainer.ExtractTextFromLeafNodes("div,span,h1", delimiter: ',');

        foreach(var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, line));
        }

        job.DescriptionHtml = await GetDescriptionElement(page,cancellationToken)
            .GetInnerHTML();

        job.Location = await jobContainer
            .QuerySelectorAsync("div[data-test-id='svx-jobview-location']")
            .GetInnerText();

        job.Location = job.Location.Split('\n').Last();


        return job;
    }

    public async Task<IElementHandle?> GetDescriptionElement(IPage page, CancellationToken cancellation)
    {
        var jobContainer = await page.QuerySelectorAsync("div[class*='jobview-container']");

        return await jobContainer
            .QuerySelectorAsync("div[class*='DescriptionContainer']");
    }
}
