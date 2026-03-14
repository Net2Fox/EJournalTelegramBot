using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Model;

public record Schedule
{
    public required DateTime Day { get; init; }

    public List<ScheduleItem> ScheduleItems { get; init; } = new List<ScheduleItem>();
}