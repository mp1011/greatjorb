namespace GreatJorb.Business.Features;

public record AddJobResultToCacheCommand(WebSite Site, JobPosting Posting) : IRequest<JobPostingCacheHeader>
{
    public class Handler : IRequestHandler<AddJobResultToCacheCommand, JobPostingCacheHeader>
    {
        private readonly LocalDataContextProvider _contextProvider;

        public Handler(LocalDataContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<JobPostingCacheHeader> Handle(AddJobResultToCacheCommand request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            JobPostingCacheHeader? header = await context.Retrieve<JobPostingCacheHeader>(request.Posting.StorageKey);

            if(header != null)
            {
                await context.Remove<JobPostingCacheHeader>(request.Posting.StorageKey);
                await context.Remove<JobPosting>(request.Posting.StorageKey);
            }

            header = new JobPostingCacheHeader
            {
                CacheDate = DateTime.Now,
                SiteUrl = request.Site.Url,
                StorageKey = request.Posting.StorageKey
            };

            await context.Store(request.Posting);
            await context.Store(header);

            return header;
        }
    }
}
