namespace GreatJorb.Business.Features;

public record class SearchJobsFromMultipleSitesQuery(JobFilter Filter) : IRequest<JobPostingSearchResult[]>
{
    public class Handler : IRequestHandler<SearchJobsFromMultipleSitesQuery, JobPostingSearchResult[]>
    {
        private readonly IMediator _mediator;
        private readonly ISettingsService _settings;

        public Handler(IMediator mediator, ISettingsService settings)
        {
            _mediator = mediator;
            _settings = settings;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsFromMultipleSitesQuery request, CancellationToken cancellationToken)
        {
            var knownJobs = new HashSet<string>();
            var sites = await _mediator.Send(new GetSitesQuery(request.Filter.Sites));

            List<JobPostingSearchResult> results = new();

            foreach(var site in sites)
            {
                results.AddRange(await _mediator.Send(new SearchJobsFromSiteCachedQuery(site, request.Filter)));
                knownJobs.AddRange(results.Select(p => p.Job.StorageKey));
            }

            int countBefore = results.Count();

            while (!cancellationToken.IsCancellationRequested) 
            {
                foreach (var site in sites)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    //if only one site is selected, don't need to switch between sites
                    int jobsToExtract = sites.Length == 1 ? int.MaxValue : _settings.JobsToExtractPerPass;

                    var siteJobs = await GetJobsFromSite(request.Filter, site, knownJobs, jobsToExtract, cancellationToken);
                    knownJobs.AddRange(siteJobs.Select(p => p.Job.StorageKey));
                    results.AddRange(siteJobs);
                }

                if (results.Count == countBefore)
                    break;
            }
            
            return results.ToArray();
        }

        private async Task<IEnumerable<JobPostingSearchResult>> GetJobsFromSite(
            JobFilter filter,
            WebSite site,
            HashSet<string> knownJobs,
            int limit,
            CancellationToken cancellationToken)
        {
            var loginResult = await _mediator.Send(new LoginQuery(site));

            return await _mediator
                .Send(new SearchJobsFromSiteQuery(loginResult.Data, filter, knownJobs, limit), cancellationToken)
                .IgnoreCancellationException(Array.Empty<JobPostingSearchResult>());
        }

        
    }
}
