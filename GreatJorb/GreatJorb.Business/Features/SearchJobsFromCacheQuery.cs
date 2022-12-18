namespace GreatJorb.Business.Features;

public record SearchJobsFromCacheQuery(WebSite Site, JobFilter Filter) : IRequest<JobPostingSearchResult>
{
    public class Handler : IRequestHandler<SearchJobsFromCacheQuery, JobPostingSearchResult>
    {
        private readonly LocalDataContextProvider _contextProvider;

        public Handler(LocalDataContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }


        public async Task<JobPostingSearchResult> Handle(SearchJobsFromCacheQuery request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            var cachedHeaders = await context.Retrieve<JobPostingCacheHeader>(p => p.SiteUrl == request.Site.Url);
            var cachedJobs = await context.Retrieve<JobPosting>(cachedHeaders.Select(p => p.StorageKey));

            JobPosting[] matches = Array.Empty<JobPosting>(), doesNotMatch = Array.Empty<JobPosting>();

            cachedJobs.SplitByCondition(p => request.Filter.IsMatch(p), ref matches, ref doesNotMatch);

            return new JobPostingSearchResult(matches, doesNotMatch);
        }
    }
}
