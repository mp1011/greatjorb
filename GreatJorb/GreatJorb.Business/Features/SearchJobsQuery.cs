namespace GreatJorb.Business.Features;

public record SearchJobsQuery(WebPage WebPage, JobFilter Filter, int NumberOfPages) 
    : IRequest<JobPostingSearchResult[]>
{

    public class Handler : IRequestHandler<SearchJobsQuery, JobPostingSearchResult[]>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            if (request.WebPage == null || request.WebPage.Page == null)
                return Array.Empty<JobPostingSearchResult>();

            IWebSiteNavigator? navigator = await _mediator.Send(new GetNavigatorQuery(request.WebPage.Site));
            if (navigator == null)
                return Array.Empty<JobPostingSearchResult>();

            IJobPostingExtractor? extractor = await _mediator.Send(new GetExtractorQuery(request.WebPage.Site));
            if (extractor == null)
                return Array.Empty<JobPostingSearchResult>();

            List<JobPosting> jobs = new();

            for (int pageNumber = 1; pageNumber <= request.NumberOfPages; pageNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                IPage? page = await navigator.GotoJobsListPage(request.WebPage.Page, request.Filter.Query, pageNumber, cancellationToken);

                page = await navigator.ApplyFilters(page, request.Filter, cancellationToken)
                    .NotifyError(page, _mediator);

                if (page == null)
                    break;

                jobs.AddRange(await extractor
                   .ExtractJobsFromPage(page, request.WebPage.Site, cancellationToken, request.Filter)
                   .NotifyError(page, _mediator, Array.Empty<JobPosting>()));
            }

            List<JobPostingSearchResult> results = new();
            foreach(var job in jobs)
            {
                string[] keywordLines = await _mediator.Send(new ExtractKeywordLinesQuery(request.Filter.Query, job.DescriptionHtml));

                results.Add(new JobPostingSearchResult(
                    job,
                    await _mediator.Send(new MatchJobFilterQuery(job, keywordLines.Any(), request.Filter)),
                    keywordLines));
            }

            return results.ToArray();
        }

       
    }
}
