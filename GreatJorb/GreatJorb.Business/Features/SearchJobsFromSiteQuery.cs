namespace GreatJorb.Business.Features;

public record SearchJobsFromSiteQuery(WebPage WebPage, JobFilter Filter, HashSet<string> KnownJobs, int Limit) 
    : IRequest<JobPostingSearchResult[]>
{

    public class Handler : IRequestHandler<SearchJobsFromSiteQuery, JobPostingSearchResult[]>
    {
        private readonly IMediator _mediator;
        private readonly ISettingsService _settings;

        public Handler(IMediator mediator, ISettingsService settingsService)
        {
            _mediator = mediator;
            _settings = settingsService;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsFromSiteQuery request, CancellationToken cancellationToken)
        {
            var page = await GoToJobsPage(request, cancellationToken);
            if (page == null)
                return Array.Empty<JobPostingSearchResult>();

            IJobPostingExtractor? extractor = await _mediator.Send(new GetExtractorQuery(request.WebPage.Site));
            if (extractor == null)
                return Array.Empty<JobPostingSearchResult>();

            List<JobPostingSearchResult> jobs = new();

            string jobsUrl = page.Url;

            HashSet<string> knownJobs = new();
            foreach(var key in request.KnownJobs)
            {
                knownJobs.Add(key);
            }

            while(!cancellationToken.IsCancellationRequested && jobs.Count < request.Limit)
            {
                var nextResult = await ExtractNextJob(page, request, knownJobs, extractor, jobsUrl, cancellationToken)
                    .WithMinimumDelay(_settings.MinTimeBetweenRequests);

                if (nextResult != null)
                {
                    knownJobs.Add(nextResult.Job.StorageKey);
                    jobs.Add(nextResult);
                }
            }

            return jobs.ToArray();
        }

        private async Task<JobPostingSearchResult?> ExtractNextJob(
            IPage page, 
            SearchJobsFromSiteQuery request, 
            HashSet<string> knownJobs,
            IJobPostingExtractor extractor, 
            string jobsUrl, 
            CancellationToken cancellationToken)
        {
            if (page.Url != jobsUrl)
            {
                await page.GoToAsync(jobsUrl);
                await page.WaitForDOMIdle(cancellationToken);
            }

            var nextJob = await extractor.ExtractNextJob(page, knownJobs, cancellationToken);
            if (nextJob == null)
                return null;

            string[] keywordLines = await _mediator.Send(new ExtractKeywordLinesQuery(request.Filter.Query, nextJob.DescriptionHtml ?? ""));

            var searchResult = new JobPostingSearchResult(
                    nextJob,
                    Site: request.WebPage.Site,
                    await _mediator.Send(new MatchJobFilterQuery(nextJob, keywordLines.Any(), request.Filter)),
                    keywordLines);

            await _mediator.Publish(new JobPostingRead(searchResult, FromCache: false));

            return searchResult;
        }

        private async Task<IPage?> GoToJobsPage(SearchJobsFromSiteQuery request, CancellationToken cancellationToken)
        {
            if (request.WebPage == null || request.WebPage.Page == null)
                return null;

            IWebSiteNavigator? navigator = await _mediator.Send(new GetNavigatorQuery(request.WebPage.Site));
            if (navigator == null)
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            IPage? page = await navigator.GotoJobsListPage(request.WebPage.Page, request.Filter.Query, cancellationToken);

            await page.WaitForDOMIdle(cancellationToken);

            page = await navigator.ApplyFilters(page, request.Filter, cancellationToken)
                .NotifyError(page, _mediator);

            if (page == null)
                return null;

            await page.WaitForDOMIdle(cancellationToken);

            return page;
        }
    }
}
