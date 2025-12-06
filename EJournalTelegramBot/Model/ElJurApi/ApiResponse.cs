using System.Text.Json.Serialization;

namespace EJournalTelegramBot.Model.ElJurApi;

public class ApiResponse<T>
{
    [JsonPropertyName("response")]
    public required ResponseData<T> Response { get; set; }
}

public class ResponseData<T>
{
    [JsonPropertyName("state")]
    public required int State { get; set; }
    
    [JsonPropertyName("error")]
    public required string Error { get; set; }
    
    [JsonPropertyName("result")]
    public required T Result { get; set; }
}