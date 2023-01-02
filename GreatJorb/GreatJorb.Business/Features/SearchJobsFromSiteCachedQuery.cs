﻿namespace GreatJorb.Business.Features;

public record SearchJobsFromSiteCachedQuery(WebSite Site, JobFilter Filter) : IRequest<JobPostingSearchResult[]>
{
    public class Handler : IRequestHandler<SearchJobsFromSiteCachedQuery, JobPostingSearchResult[]>
    {
        private readonly LocalDataContextProvider _contextProvider;
        private readonly IMediator _mediator;

        public Handler(LocalDataContextProvider contextProvider, IMediator mediator)
        {
            _contextProvider = contextProvider;
            _mediator = mediator;
        }

        public async Task<JobPostingSearchResult[]> Handle(SearchJobsFromSiteCachedQuery request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            var cachedHeaders = await context.Retrieve<JobPostingCacheHeader>(p => p.SiteUrl == request.Site.Url);
            var cachedJobs = await context.Retrieve<JobPosting>(cachedHeaders.Select(p => p.StorageKey));

            List<JobPostingSearchResult> results = new();
            foreach(var job in cachedJobs)
            {
                var keywordLines = await _mediator.Send(new ExtractKeywordLinesQuery(request.Filter.Query, job.DescriptionHtml ?? ""));

                var result = new JobPostingSearchResult(
                    Job: job,
                    FilterMatches: await _mediator.Send(new MatchJobFilterQuery(job, keywordLines.Any(), request.Filter)),
                    KeywordLines: keywordLines);

                results.Add(result);

                await _mediator.Publish(new JobPostingRead(job, request.Site, FromCache:true));
            }

            return results.ToArray();
        }
    }
}