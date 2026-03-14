using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public record ScheduleResult
{
    [JsonPropertyName("name")]
    public required string Day { get; init; }
    
    [JsonPropertyName("title")]
    public required string DayOfWeek { get; init; }
    
    [JsonPropertyName("items")]
    public required List<ScheduleItem> ScheduleItems { get; init; }
}