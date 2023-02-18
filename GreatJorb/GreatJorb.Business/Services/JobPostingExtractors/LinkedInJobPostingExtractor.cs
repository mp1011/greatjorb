namespace GreatJorb.Business.Services.JobPostingExtractors;

public class LinkedInJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;
    private readonly ISettingsService _settings;

    public string WebsiteName => "LinkedIn";


    public LinkedInJobPostingExtractor(IMediator mediator, ISettingsService settings)
    {
        _mediator = mediator;
        _settings = settings;
    }


    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, JobFilter filter, HashSet<string> knownJobs, int Limit, CancellationToken cancellationToken)
    {     
        var jobCards = await page.QuerySelectorAllAsync(".job-card-container");

        throw new NotImplementedException();
        //if (PageSize != null)
        //    jobCards = jobCards.Take(PageSize.Value).ToArray();

        List<JobPosting> postingHeaders = new();

        foreach(var jobCard in jobCards)
        {
            postingHeaders.Add(await ExtractPostingHeaders(jobCard));
        }

        List<JobPosting> postingDetails = new();
        foreach(var header in postingHeaders)
        {
            cancellationToken.ThrowIfCancellationRequested();
            postingDetails.Add(await ExtractPostingDetails(page, header, filter)
                .WithMinimumDelay(_settings.MinTimeBetweenRequests));            
        }

        return postingDetails.ToArray();
    }

    private async Task<JobPosting> ExtractPostingHeaders(IElementHandle jobCard)
    {
        string url = await jobCard
                .QuerySelectorAsync("a.job-card-container__link")
                .GetAttribute("href");

        url = $"https://linkedin.com/{url}";

        return new JobPosting(url, new Uri(url).GetLeftPart(UriPartial.Path))
        {
            Title = await jobCard.GetTextAsync(".job-card-list__title"),
            Company = await jobCard.GetTextAsync(".job-card-container__company-name"),
           
            WorkplaceType = await jobCard
                .GetTextAsync(".job-card-container__metadata-item--workplace-type")
                .TryParseEnumAdvanced<WorkplaceType>(),

            JobType = JobType.Unknown,
            SalaryType = SalaryType.Unknown
        };
    }
    
    private async Task<JobPosting> ExtractPostingDetails(IPage page, JobPosting posting, JobFilter filter)
    {
        await page.GoToAsync(posting.Uri.ToString());

        await page.WaitForSelectorAsync(".jobs-unified-top-card__job-insight");

        var metaData = await page
            .QuerySelectorAllAsync(".job-card-container__metadata-item")
            .GetInnerTextAsync();

        var insights = await page
            .QuerySelectorAllAsync(".jobs-unified-top-card__job-insight")
            .GetInnerTextAsync();

        var description = await page
            .QuerySelectorAsync(".jobs-description-content")
            .GetInnerHTML();

        posting.Company = await page.GetInnerTextAsync(".jobs-unified-top-card__company-name");

        posting.Location = await page.GetInnerTextAsync(".jobs-unified-top-card__company-name + span");

        posting.WorkplaceType = await page
            .GetInnerTextAsync(".jobs-unified-top-card__workplace-type")
            .TryParseEnumAdvanced<WorkplaceType>();

        foreach(string insight in insights)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(posting, insight));
        }
        
        posting.DescriptionHtml = description;

        posting.MiscProperties = metaData
            .Union(insights)
            .ToArray();

        await _mediator.Send(new SetPropertiesFromTextCommand(posting, posting.Title ?? ""));

        throw new Exception("handle this differently");
       //await _mediator.Publish(new JobPostingRead(posting, site, FromCache: false));

        return posting;       
    }

}
