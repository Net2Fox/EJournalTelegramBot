using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public record ApiResponse<T>
{
    [JsonPropertyName("response")]
    public required ResponseData<T> Response { get; init; }
}

public record ResponseData<T>
{
    [JsonPropertyName("state")]
    public required int State { get; init; }
    
    [JsonPropertyName("error")]
    public required string Error { get; init; }
    
    [JsonPropertyName("result")]
    public required T Result { get; init; }
}