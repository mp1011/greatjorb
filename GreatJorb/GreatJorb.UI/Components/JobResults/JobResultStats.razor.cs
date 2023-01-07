namespace GreatJorb.UI.Components.JobResults;

public partial class JobResultStats
{
    [Parameter]
    public List<JobPostingSearchResult> Postings { get; set; } = new();

    public WebSite[] Sites =>
        Postings
                .Select(p => p.Site)
                .Distinct()
                .ToArray();

    public int GetCountForSite(WebSite site) =>
        Postings.Count(p => p.Site == site);
}
