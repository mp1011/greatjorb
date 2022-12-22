namespace GreatJorb.Business.Features;

public record SearchJobsFromCacheQuery(WebSite Site, JobFilter Filter) : IRequest<JobPostingSearchResult[]>
{
    public class Handler : IRequestHandler<SearchJobsFromCacheQuery, JobPostingSearchResult[]>
    {
        private readonly LocalDataContextProvider _contextProvider;
        private readonly IMediator _mediator;

        public Handler(LocalDataContextProvider contextProvider, IMediator mediator)
        {
            _contextProvider = contextProvider;
            _mediator = mediator;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsFromCacheQuery request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            var cachedHeaders = await context.Retrieve<JobPostingCacheHeader>(p => p.SiteUrl == request.Site.Url);
            var cachedJobs = await context.Retrieve<JobPosting>(cachedHeaders.Select(p => p.StorageKey));

            List<JobPostingSearchResult> results = new();
            foreach(var job in cachedJobs)
            {
                results.Add(new JobPostingSearchResult(job, await _mediator.Send(new MatchJobFilterQuery(job, request.Filter))));
            }

            return results.ToArray();
        }
    }
}
