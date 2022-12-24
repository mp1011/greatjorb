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
                var keywordLines = await _mediator.Send(new ExtractKeywordLinesQuery(request.Filter.Query, job.DescriptionHtml ?? ""));

                results.Add(new JobPostingSearchResult(
                    Job: job, 
                    FilterMatches: await _mediator.Send(new MatchJobFilterQuery(job, keywordLines.Any(), request.Filter)),
                    KeywordLines: keywordLines));
            }

            return results.ToArray();
        }
    }
}
