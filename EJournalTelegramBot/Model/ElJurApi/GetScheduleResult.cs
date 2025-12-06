using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public class GetScheduleResult
{
    [JsonPropertyName("name")]
    public required string Day { get; set; }
    
    [JsonPropertyName("title")]
    public required string DayOfWeek { get; set; }
    
    [JsonPropertyName("items")]
    public List<ScheduleItem> ScheduleItems { get; set; }
}