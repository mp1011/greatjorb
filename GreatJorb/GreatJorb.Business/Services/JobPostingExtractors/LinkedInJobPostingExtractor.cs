﻿using GreatJorb.Business.Features;

namespace GreatJorb.Business.Services.JobPostingExtractors;

public class LinkedInJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => "LinkedIn";


    public LinkedInJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int? PageSize = null)
    {
        var jobCards = await page.QuerySelectorAllAsync(".job-card-container");

        if (PageSize != null)
            jobCards = jobCards.Take(PageSize.Value).ToArray();

        List<JobPosting> postingHeaders = new();

        foreach(var jobCard in jobCards)
        {
            postingHeaders.Add(await ExtractPostingHeaders(jobCard));
        }

        List<JobPosting> postingDetails = new();
        foreach(var header in postingHeaders)
        {
            postingDetails.Add(await ExtractPostingDetails(page, header));
        }

        return postingDetails.ToArray();
    }

    private async Task<JobPosting> ExtractPostingHeaders(IElementHandle jobCard)
    {
        return new JobPosting
        {
            Url = await jobCard
                .QuerySelectorAsync("a.job-card-container__link")
                .GetAttribute("href"),
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
        await page.GoToAsync($"https://linkedin.com/{posting.Url}");

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

        if(insights.Any())
        {
            string[] insightsElements = insights[0].Split(" · ", StringSplitOptions.TrimEntries);
            if (insightsElements.Length >= 2)
            {
                posting.JobType = insightsElements[^2].TryParseEnumAdvanced(JobType.Unknown);
                posting.JobLevel = insightsElements[^1].TryParseEnumAdvanced(JobLevel.Unknown);
            }

            foreach(var element in insightsElements)
            {
                var salaryFromElement = await _mediator.Send(new ParseSalaryQuery(element));
                if(salaryFromElement.Min != null || salaryFromElement.Max != null)
                {
                    posting.SalaryMin = salaryFromElement.Min;
                    posting.SalaryMax = salaryFromElement.Max;
                    posting.SalaryType = salaryFromElement.SalaryType;
                }
            }
        }

        posting.DescriptionHtml = description;

        posting.MiscProperties = metaData
            .Union(insights)
            .ToArray();

        return posting;       
    }
}