namespace GreatJorb.Business.Models;

public class JobPostingCacheHeader : ILocalStorable
{
    public string SiteUrl { get; set; } = "";

    public DateTime CacheDate { get; set; }

    public string StorageKey { get; set; } = "";
}
