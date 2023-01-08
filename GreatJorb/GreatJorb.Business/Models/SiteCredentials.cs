namespace GreatJorb.Business.Models;

public class SiteCredentials : ILocalStorable
{
    public SiteCredentials(string site, string userName, string password)
    {
        Site = site;
        UserName = userName;
        Password = password;
    }

    public string Site { get; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string StorageKey => Site;
}
