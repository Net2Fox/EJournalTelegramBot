using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public record ScheduleItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("num")]
    public string Num { get; init; }
    
    [JsonPropertyName("room")]
    public string Room { get; init; }
    
    [JsonPropertyName("teacher")]
    public string Teacher { get; init; }
    
    [JsonPropertyName("starttime")]
    public string StartTime { get; init; }
    
    [JsonPropertyName("endtime")]
    public string EndTime { get; init; }
    
    [JsonPropertyName("grp")]
    public string? SubGroup { get; init; }
}