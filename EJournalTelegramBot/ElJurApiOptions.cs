namespace EJournalTelegramBot;

public class ElJurApiOptions
{
    public string BaseUrl { get; }
    public string DevKey { get; }
    public string AuthToken { get; }
    public string Vendor { get; }

    public ElJurApiOptions(string baseUrl, string devKey, string authToken, string vendor)
    {
        BaseUrl = baseUrl;
        DevKey = devKey;
        AuthToken = authToken;
        Vendor = vendor;
    }
}