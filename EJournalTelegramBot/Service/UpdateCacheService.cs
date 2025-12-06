using EJournalTelegramBot.Model;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class UpdateCacheService(ElJurApiService elJurApi, CacheService cacheService)
{
    public async Task UpdateCache()
    {
        var tomorrow = DateTime.Now;
        switch (tomorrow.DayOfWeek)
        {
            case DayOfWeek.Friday:
                tomorrow = tomorrow.AddDays(3);
                break;
            case DayOfWeek.Saturday:
                tomorrow = tomorrow.AddDays(2);
                break;
            case DayOfWeek.Sunday:
                tomorrow = tomorrow.AddDays(1);
                break;
            default:
                tomorrow = tomorrow.AddDays(1);
                break;
        }
        
        GetScheduleResult? scheduleResult = await elJurApi.GetSchedule("3ИСИП-323", tomorrow.ToString("yyyyMMdd"), "yes");
        if (scheduleResult != null)
        {
            var schedule = new Schedule()
            {
                Day = tomorrow,
                ScheduleItems = scheduleResult.ScheduleItems
            };
            cacheService.UpdateSchedule("3ИСИП-323", schedule);
        }
    }
}