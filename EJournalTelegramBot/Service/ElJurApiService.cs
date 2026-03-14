using System.Text.Json;
using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class ElJurApiService
{
    private readonly ElJurApiConfiguration _options;
    
    private readonly HttpClient _httpClient;
    
    private string BaseUrl => _options.BaseUrl;
    private string DevKey =>  _options.DevKey;
    private string AuthToken => _options.AuthToken;
    private string Vendor => _options.Vendor;

    public ElJurApiService(ElJurApiConfiguration options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? new HttpClient();
    }
    
    public async Task<ScheduleResult?> GetSchedule(string groupName, string date, string rings)
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
            ScheduleResult? getScheduleResult = data.Deserialize<ScheduleResult>();
            return getScheduleResult;
        }
        return null;
    }
    
    public async Task<GroupsResult?> GetGroups()
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
            GroupsResult groupsResult = new GroupsResult();
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
    
    public async Task<List<string>?> GetTeachers()
    {
        var url = $"{BaseUrl}/getmessagereceivers?devkey={DevKey}&out_format=json&vendor={Vendor}&auth_token={AuthToken}";
        HttpResponseMessage httpResponse = await _httpClient.GetAsync(url);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception("Error getting schedule");
        }
        var jsonContent =  await httpResponse.Content.ReadAsStringAsync();
        
        var teachers = new HashSet<string>();
        using JsonDocument document = JsonDocument.Parse(jsonContent);
        JsonElement root = document.RootElement;
        if (root.TryGetProperty("response", out JsonElement response) &&
            response.TryGetProperty("result", out JsonElement result) &&
            result.TryGetProperty("groups", out JsonElement groups))
        {
            foreach (JsonElement group in groups.EnumerateArray())
            {
                if (group.TryGetProperty("key", out JsonElement keyElement) &&
                    keyElement.GetString() == "categories" &&
                    group.TryGetProperty("subgroups", out JsonElement subgroups))
                {
                    foreach (JsonElement subgroup in subgroups.EnumerateArray())
                    {
                        if (subgroup.TryGetProperty("name", out JsonElement nameElement) &&
                            nameElement.GetString() == "Преподаватель" &&
                            subgroup.TryGetProperty("users", out JsonElement users))
                        {
                            foreach (JsonElement userElement in users.EnumerateArray())
                            {
                                if (userElement.TryGetProperty("lastname", out JsonElement lastnameEl)
                                    && userElement.TryGetProperty("firstname", out JsonElement firstnameEl))
                                {
                                    string FIO = $"{lastnameEl.GetString().Trim()} {firstnameEl.GetString().Trim()} {(userElement.TryGetProperty("middlename", out JsonElement middlenameEl) ? middlenameEl.GetString().Trim() : null)}";
                                    teachers.Add(FIO);
                                }
                            }
                        }
                    }
                }
                
                if (keyElement.GetString() == "teachers" &&
                    group.TryGetProperty("users", out JsonElement usersElement))
                {
                    foreach (JsonElement user in usersElement.EnumerateArray())
                    {
                        if (user.TryGetProperty("lastname", out JsonElement lastnameEl)
                            && user.TryGetProperty("firstname", out JsonElement firstnameEl))
                        {
                            string FIO = $"{lastnameEl.GetString().Trim()} {firstnameEl.GetString().Trim()} {(user.TryGetProperty("middlename", out JsonElement middlenameEl) ? middlenameEl.GetString().Trim() : null)}";
                            teachers.Add(FIO);
                        }
                    }
                }
            }
        }

        return teachers.Count switch
        {
            0 => null,
            _ => teachers.OrderBy(t => t).ToList()

        };
    }
}