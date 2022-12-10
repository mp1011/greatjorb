﻿namespace GreatJorb.Business.Services;

public interface ISettingsService
{
    string GetSiteUserName(WebSite site);
    string GetSitePassword(WebSite site);
    void SetSiteUserName(WebSite site, string value);
    void SetSitePassword(WebSite site, string value);
}


[SupportedOSPlatform("windows")]
public class SettingsService : ISettingsService
{
    private readonly IConfiguration _configuration;

    public SettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetSiteUserName(WebSite site) => GetSecureText($"{site.Name}.UserName");

    public void SetSiteUserName(WebSite site, string value) => SetSecureText($"{site.Name}.UserName", value);

    public string GetSitePassword(WebSite site) => GetSecureText($"{site.Name}.Password");

    public void SetSitePassword(WebSite site, string value) => SetSecureText($"{site.Name}.Password", value);


    //fixed random number to assist with encryption. Not required to be perfectly secret.
    private byte[] GetEntropy(string key)
    {
        byte[] rng = new byte[16];
        new Random(849382).NextBytes(rng);

        var keyBytes = UnicodeEncoding.ASCII.GetBytes(key);
        return rng.Union(keyBytes).ToArray();
    }

    private string GetSecureText(string key)
    {
        if (key == null)
            return string.Empty;

        try
        {
            var encrypted = Convert.FromBase64String(_configuration[key]!);
            var decrypted = ProtectedData.Unprotect(encrypted, GetEntropy(key), DataProtectionScope.LocalMachine);
            return UnicodeEncoding.ASCII.GetString(decrypted);
        }
        catch
        {
            return String.Empty;
        }
    }

    private void SetSecureText(string key, string value)
    {
        var textBytes = UnicodeEncoding.ASCII.GetBytes(value);
        var encrypyted = ProtectedData.Protect(textBytes, GetEntropy(key), DataProtectionScope.LocalMachine);

        var encryptedText = Convert.ToBase64String(encrypyted);
        _configuration[key] = encryptedText;
    }
}