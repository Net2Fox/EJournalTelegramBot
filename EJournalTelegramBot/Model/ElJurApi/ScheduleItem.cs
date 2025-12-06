using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public class ScheduleItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("num")]
    public string Num { get; set; }
    
    [JsonPropertyName("room")]
    public string Room { get; set; }
    
    [JsonPropertyName("teacher")]
    public string Teacher { get; set; }
    
    [JsonPropertyName("starttime")]
    public string StartTime { get; set; }
    
    [JsonPropertyName("endtime")]
    public string EndTime { get; set; }
    
    [JsonPropertyName("grp")]
    public string? SubGroup { get; set; }
}