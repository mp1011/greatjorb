namespace GreatJorb.Business.Features.Notifications;

public record JobPostingRead(JobPosting Job, WebSite Site) : INotification { }
