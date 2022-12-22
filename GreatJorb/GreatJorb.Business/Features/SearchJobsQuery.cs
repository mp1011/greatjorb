namespace GreatJorb.Business.Features;

public record SearchJobsQuery(WebPage WebPage, JobFilter Filter, int NumberOfPages) 
    : IRequest<JobPostingSearchResult>
{

    public class Handler : IRequestHandler<SearchJobsQuery, JobPostingSearchResult>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JobPostingSearchResult> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            if (request.WebPage == null || request.WebPage.Page == null)
                return JobPostingSearchResult.Empty;

            IWebSiteNavigator? navigator = await _mediator.Send(new GetNavigatorQuery(request.WebPage.Site));
            if (navigator == null)
                return JobPostingSearchResult.Empty;

            IJobPostingExtractor? extractor = await _mediator.Send(new GetExtractorQuery(request.WebPage.Site));
            if (extractor == null)
                return JobPostingSearchResult.Empty;

            List<JobPosting> results = new();

            for (int pageNumber = 1; pageNumber <= request.NumberOfPages; pageNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                IPage? page = await navigator.GotoJobsListPage(request.WebPage.Page, request.Filter.Query, pageNumber, cancellationToken);

                page = await navigator.ApplyFilters(page, request.Filter, cancellationToken)
                    .NotifyError(page, _mediator);

                if (page == null)
                    break;

                results.AddRange(await extractor
                   .ExtractJobsFromPage(page, request.WebPage.Site, cancellationToken)
                   .NotifyError(page, _mediator, Array.Empty<JobPosting>()));
            }

            JobPosting[] matchesFilter = Array.Empty<JobPosting>(), doesNotMatchFilter = Array.Empty<JobPosting>();

            results.SplitByCondition(p => request.Filter.IsMatch(p), ref matchesFilter, ref doesNotMatchFilter);

            return new JobPostingSearchResult(matchesFilter, doesNotMatchFilter);          
        }

       
    }
}
