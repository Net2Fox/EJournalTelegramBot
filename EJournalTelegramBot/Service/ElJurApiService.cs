using System.Text.Json;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class ElJurApiService
{
    private readonly ElJurApiOptions _options;
    
    private readonly HttpClient _httpClient;
    
    private string BaseUrl => _options.BaseUrl;
    private string DevKey =>  _options.DevKey;
    private string AuthToken => _options.AuthToken;
    private string Vendor => _options.Vendor;

    public ElJurApiService(ElJurApiOptions options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? new HttpClient();
    }
    
    public async Task<GetScheduleResult?> GetSchedule(string groupName, string date, string rings)
    {
        var url = $"{BaseUrl}/getschedule?class={groupName}&days={date}&rings={rings}&devkey={DevKey}&out_format=json&vendor={Vendor}&auth_token={AuthToken}";
        
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error getting schedule!");
        }
        
        var json =  await response.Content.ReadAsStringAsync();
        
        using JsonDocument document = JsonDocument.Parse(json);
        
        JsonElement root = document.RootElement;

        if (root.TryGetProperty("response", out JsonElement jsonResponse) &&
            jsonResponse.TryGetProperty("result", out JsonElement result) &&
            result.TryGetProperty("days", out JsonElement days) &&
            days.TryGetProperty(date, out JsonElement data))
        {
            GetScheduleResult? getScheduleResult = data.Deserialize<GetScheduleResult>();
            return getScheduleResult;
        }
        return null;
    }
    
    public async Task<GetGroupsResult?> GetGroups()
    {
        var url = $"{BaseUrl}/getmessagereceivers?devkey={DevKey}&out_format=json&vendor={Vendor}&auth_token={AuthToken}";
        
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error getting schedule");
        }
        
        var json =  await response.Content.ReadAsStringAsync();
        
        using JsonDocument document = JsonDocument.Parse(json);
        
        JsonElement root = document.RootElement;

        if (root.TryGetProperty("response", out JsonElement jsonResponse) &&
            jsonResponse.TryGetProperty("result", out JsonElement result) &&
            result.TryGetProperty("groups", out JsonElement groups))
        {
            GetGroupsResult groupsResult = new GetGroupsResult();
            foreach (var group in groups.EnumerateArray())
            {
                if (group.TryGetProperty("key", out JsonElement keyElement) &&
                    keyElement.GetString() == "students" &&
                    group.TryGetProperty("subgroups", out JsonElement subgroups))
                {
                    foreach (var subGroup in subgroups.EnumerateArray())
                    {
                        if (subGroup.TryGetProperty("name", out JsonElement nameElement))
                        {
                            groupsResult.Groups.Add(nameElement.GetString());
                        }
                    }
                }
            }
            return groupsResult;
        }
        return null;
    }
}