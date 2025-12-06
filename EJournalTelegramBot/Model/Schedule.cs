using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Model;

public class Schedule
{
    public required DateTime Day { get; set; }
    
    public List<ScheduleItem> ScheduleItems { get; set; }
}