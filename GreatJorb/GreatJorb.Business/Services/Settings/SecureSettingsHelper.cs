namespace GreatJorb.Business.Services.Settings;

[SupportedOSPlatform("windows")]
public static class SecureSettingsHelper
{
    //fixed random number to assist with encryption. Not required to be perfectly secret.
    private static byte[] GetEntropy(string key)
    {
        byte[] rng = new byte[16];
        new Random(849382).NextBytes(rng);

        var keyBytes = Encoding.ASCII.GetBytes(key);
        return rng.Union(keyBytes).ToArray();
    }

    public static string ReadSecureText(string key, string base64Encrypted)
    {
        try
        {
            var decoded = Convert.FromBase64String(base64Encrypted);
            var decrypted = ProtectedData.Unprotect(decoded, GetEntropy(key), DataProtectionScope.LocalMachine);
            return Encoding.ASCII.GetString(decrypted);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string EncryptText(string key, string value)
    {
        var textBytes = Encoding.ASCII.GetBytes(value);
        var encrypyted = ProtectedData.Protect(textBytes, GetEntropy(key), DataProtectionScope.LocalMachine);

        return Convert.ToBase64String(encrypyted);
    }
}
