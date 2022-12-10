namespace GreatJorb.Business.Features;

public record SearchJobsQuery(WebPage WebPage, string Query, int NumberOfPages) 
    : IRequest<JobPosting[]>
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

            List<JobPosting> results = new();

            for (int pageNumber = 1; pageNumber <= request.NumberOfPages; pageNumber++)
            {
                IPage page = await navigator.GotoJobsListPage(request.WebPage.Page, request.Query, pageNumber);

                results.AddRange(await extractor
                   .ExtractJobsFromPage(page)
                   .HandleError(error =>
                   {
                        //log error or something
                       return Task.FromResult(Array.Empty<JobPosting>());
                   }));
            }
          
            return results.ToArray();
        }
    }
}
