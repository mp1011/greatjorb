namespace GreatJorb.UI.Components.JobResults
{
    public partial class JobResultPreview
    {
        public record JobBadge(string Text, bool? IsMatch, bool NotPartOfFilter);

        [Parameter]
        public JobPostingSearchResult JobResult { get; set; }

        public JobPosting Job => JobResult.Job;


        public async Task CopyUrl()
        {
            await Clipboard.SetTextAsync(Job.Uri.ToString());
        }
      
        public IEnumerable<JobBadge> GetBadges()
        {
            var workplaceTypeBadge = GetBadge(nameof(JobFilter.WorkplaceTypeFilter), "Workplace Type", JobResult.Job.WorkplaceType, p => p == WorkplaceType.Unknown);
            if (workplaceTypeBadge != null)
                yield return workplaceTypeBadge;

            var salaryTypeBadge = GetBadge(nameof(JobFilter.SalaryTypeFilter), "Salary Type", JobResult.Job.SalaryType, p => p == SalaryType.Unknown);
            if (salaryTypeBadge != null)
                yield return salaryTypeBadge;

            var jobLevelBadge = GetBadge(nameof(JobFilter.JobLevelFilter), "Job Level", JobResult.Job.JobLevel, p => p == JobLevel.Unknown);
            if (jobLevelBadge != null)
                yield return jobLevelBadge;

            var jobTypeBadge = GetBadge(nameof(JobFilter.JobTypeFilter), "Job Type", JobResult.Job.JobType, p => p == JobType.Unknown);
            if (jobTypeBadge != null)
                yield return jobTypeBadge;

            var salaryFilter = JobResult.FilterMatches.FirstOrDefault(p => p.Field == nameof(JobFilter.Salary));
            if(salaryFilter != null)
            {
                yield return salaryFilter.Level switch
                {
                    FilterMatchLevel.PositiveMatch => new JobBadge("Salary Matches Filter", true, false),
                    FilterMatchLevel.NegativeMatch => new JobBadge("Salary Does Not Match Filter", IsMatch: false, NotPartOfFilter: false),
                    _ => new JobBadge("Salary not Provided", IsMatch: null, NotPartOfFilter: false),
                };
            }

            if (!JobResult.KeywordLines.Any(p=>p.Type == KeywordLineType.Query))                
                yield return new JobBadge("Description does not contain keywords", false, false);
        }

        private JobBadge GetBadge<T>(string fieldName, string friendlyFieldName, T value, Func<T,bool> isEmpty)
            where T:Enum
        {
            var filter = JobResult.FilterMatches.FirstOrDefault(p => p.Field == fieldName);

            string text = (isEmpty(value)) ? $"{friendlyFieldName} not provided" : value.GetDisplayName();

            if(filter == null)
            {
                if (isEmpty(value))
                    return null;
                else 
                    return new JobBadge(text, false, true);
            }

            return filter.Level switch
            {
                FilterMatchLevel.PositiveMatch => new JobBadge(text, true, false),
                FilterMatchLevel.NegativeMatch => new JobBadge(text, false, false),
                _ => new JobBadge(text, null, false),
            };
        }
    }
}
