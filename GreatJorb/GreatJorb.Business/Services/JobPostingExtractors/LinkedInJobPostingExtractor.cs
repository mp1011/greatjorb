﻿namespace GreatJorb.Business.Services.JobPostingExtractors;

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


    public string GetStorageKeyFromUrl(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var jobCards = await page.QuerySelectorAllAsync(".job-card-container");

        foreach(var jobCard in jobCards)
        {
            string url = await jobCard
                .QuerySelectorAsync("a.job-card-container__link")
                .GetAttribute("href");

            if (knownJobs.Contains(GetStorageKeyFromUrl(url)))
                continue;

            var header = await ExtractPostingHeaders(jobCard);
            return await ExtractPostingDetails(page, header);
        }

        return null;
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
    
    private async Task<JobPosting> ExtractPostingDetails(IPage page, JobPosting posting)
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
