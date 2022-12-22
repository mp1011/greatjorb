namespace GreatJorb.UI.Components.JobResults
{
    public partial class JobResultPreview
    {
        [Parameter]
        public JobPostingSearchResult JobResult { get; set; }

        public JobPosting Job => JobResult.Job;

        public string JobLevelCss => JobResult.Job.JobLevel switch
        {
            JobLevel.Unknown => "d-none",
            JobLevel.SeniorLevel => "bg-success",
            JobLevel.MidSeniorLevel => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string WorkplaceTypeCss => JobResult.Job.WorkplaceType switch
        {
            WorkplaceType.Unknown => "d-none",
            WorkplaceType.Remote => "bg-success",
            WorkplaceType.Hybrid => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string JobTypeCss => JobResult.Job.JobType switch
        {
            JobType.Unknown => "d-none",
            JobType.FullTime => "bg-success",
            JobType.Contract => "bg-warning text-dark",
            _ => "bg-danger"
        };
    }
}
