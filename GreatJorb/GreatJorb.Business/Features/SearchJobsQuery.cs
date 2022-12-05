namespace GreatJorb.Business.Features;

public record SearchJobsQuery(WebPage WebPage, string Query) : IRequest<JobPosting[]>
{

    public class Handler : IRequestHandler<SearchJobsQuery, JobPosting[]>
    {
        private readonly IMediator _mediator;
        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JobPosting[]> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            if (request.WebPage == null || request.WebPage.Page == null)
                return Array.Empty<JobPosting>();

            IWebSiteNavigator? navigator = await _mediator.Send(new GetNavigatorQuery(request.WebPage.Site));
            if (navigator == null)
                return Array.Empty<JobPosting>();

            IJobPostingExtractor? extractor = await _mediator.Send(new GetExtractorQuery(request.WebPage.Site));
            if (extractor == null)
                return Array.Empty<JobPosting>();

            IPage page = await navigator.GotoJobsListPage(request.WebPage.Page, request.Query);
            return await extractor.ExtractJobsFromPage(page);
        }
    }
}
