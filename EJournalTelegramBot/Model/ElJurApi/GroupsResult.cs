namespace EJournalTelegramBot.Model.ElJurApi;

public record GroupsResult
{
    public List<string> Groups { get; init; } = new List<string>();
}