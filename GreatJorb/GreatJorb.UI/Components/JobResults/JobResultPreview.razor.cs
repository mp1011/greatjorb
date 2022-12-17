namespace GreatJorb.UI.Components.JobResults
{
    public partial class JobResultPreview
    {
        [Parameter]
        public JobPosting Job { get; set; }

        public string JobLevelCss => Job.JobLevel switch
        {
            JobLevel.SeniorLevel => "bg-success",
            JobLevel.MidSeniorLevel => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string WorkplaceTypeCss => Job.WorkplaceType switch
        {
            WorkplaceType.Remote => "bg-success",
            WorkplaceType.Hybrid => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string JobTypeCss => Job.JobType switch
        {
            JobType.FullTime => "bg-success",
            JobType.Contract => "bg-warning text-dark",
            _ => "bg-danger"
        };
    }
}
