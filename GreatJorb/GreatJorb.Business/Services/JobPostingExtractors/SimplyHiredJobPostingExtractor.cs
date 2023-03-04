namespace GreatJorb.Business.Services.JobPostingExtractors;

public class SimplyHiredJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public SimplyHiredJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string WebsiteName => Site.SimplyHired.GetDisplayName();

    public string GetStorageKeyFromUrl(string url)
    {
        return url
            .SubstringUpTo('?')
            .Split('/')
            .Last();
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var cards = await page.QuerySelectorAllAsync("#job-list li");

        foreach (var card in cards)
        {
            var url = await card.QuerySelectorAsync("a").GetAttribute("href");

            if (url.IsNullOrEmpty()
                    || !url.Contains("/job")
                    || knownJobs.Contains(GetStorageKeyFromUrl(url)))
            {
                continue;
            }

            await page.GoToAsync(url);
            return await ExtractJob(page, cancellationToken);
        }

        return null;
    }
    public async Task<bool> GotoNextPage(IPage page, CancellationToken cancellationToken)
    {
        var pager = await page.WaitForSelectorAsync("nav.pagination ul");
        var pagerElements = await pager.QuerySelectorAllAsync("li");

        bool foundSelected = false;

        foreach (var element in pagerElements)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!foundSelected)
            {
                var classes = (await element.GetAttribute("class")).Split(' ');
                foundSelected = classes.Contains("active");
            }
            else
            {
                await page.ClickAndWaitForNavigation(element);
                return true;
            }
        }

        return false;
    }

    private async Task<JobPosting> ExtractJob(IPage page, CancellationToken cancellationToken)
    {
        var job = new JobPosting();

        job.Uri = new Uri(page.Url);
        job.StorageKey = GetStorageKeyFromUrl(page.Url);

        job.Title = await page
            .QuerySelectorAsync("h2[data-testid='viewJobTitle']")
            .GetInnerText();

        job.Company = await page
            .QuerySelectorAsync("span[data-testid='viewJobCompanyName']")
            .GetInnerText();

        var rightPanel = await page.QuerySelectorAsync("aside");

        var textList = await rightPanel.ExtractTextFromLeafNodes("span");

        foreach(var text in textList)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, text));
        }

        job.DescriptionHtml = await page
            .QuerySelectorAsync("div[data-testid='viewJobBodyJobFullDescriptionContent']")
            .GetInnerHTML();

        return job;
    }

    public async Task<IElementHandle?> GetDescriptionElement(IPage page, CancellationToken cancellation)
    {
        return await page
            .QuerySelectorAsync("div[data-testid='viewJobBodyJobFullDescriptionContent']");
    }
}
