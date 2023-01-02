namespace GreatJorb.Business.Features;

public record class SearchJobsFromMultipleSitesQuery(JobFilter Filter, int NumPages) : IRequest<JobPostingSearchResult[]>
{
    public class Handler : IRequestHandler<SearchJobsFromMultipleSitesQuery, JobPostingSearchResult[]>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsFromMultipleSitesQuery request, CancellationToken cancellationToken)
        {
            var sites = GetSites(request.Filter.Sites);

            List<JobPostingSearchResult> results = new();

            foreach(var site in sites)
            {
                results.AddRange(await _mediator.Send(new SearchJobsFromSiteCachedQuery(site, request.Filter)));
            }

            int page = 0;
            while(++page <= request.NumPages)
            {
                foreach(var site in sites)
                {
                    results.AddRange(await GetJobsFromSite(request.Filter, site, page, cancellationToken));
                }
            }

            return results.ToArray();
        }

        private async Task<IEnumerable<JobPostingSearchResult>> GetJobsFromSite(
            JobFilter filter,
            WebSite site,
            int pageNumber,
            CancellationToken cancellationToken)
        {
            var loginResult = await _mediator.Send(new LoginQuery(site));
           
            return await _mediator
                .Send(new SearchJobsFromSiteQuery(loginResult.Data, filter, pageNumber), cancellationToken)
                .IgnoreCancellationException(Array.Empty<JobPostingSearchResult>());
        }

        private IEnumerable<WebSite> GetSites(Site sites)
        {
            if(sites.HasFlag(Site.LinkedIn))
            {
                yield return new WebSite("LinkedIn", "https://www.linkedin.com/");
            }

            if (sites.HasFlag(Site.GoogleJobs))
            {
                yield return new WebSite("Google Jobs", "https://www.google.com/");
            }
        }
    }
}
