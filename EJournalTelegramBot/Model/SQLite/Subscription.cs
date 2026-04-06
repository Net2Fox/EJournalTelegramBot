namespace EJournalTelegramBot.Model.SQLite;

public class Subscription
{
    public int Id { get; set; }
    public long ChatId { get; set; }
    public required string Prefix { get; set; }
    public required string Value { get; set; }
}