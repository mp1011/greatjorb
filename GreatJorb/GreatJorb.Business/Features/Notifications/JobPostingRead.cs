namespace GreatJorb.Business.Features.Notifications;

public record JobPostingRead(JobPostingSearchResult JobSearchResult, bool FromCache) : INotification { }
