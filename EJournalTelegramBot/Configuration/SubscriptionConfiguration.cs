namespace EJournalTelegramBot.Configuration;

public class SubscriptionConfiguration
{
    public long ChatId { get; init; }
    public List<string>? Subscriptions { get; init; } = null!;
}