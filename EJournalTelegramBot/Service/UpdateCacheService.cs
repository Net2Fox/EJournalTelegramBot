using EJournalTelegramBot.Model;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class UpdateCacheService(ElJurApiService elJurApi, CacheService cacheService, ILogger<UpdateCacheService> logger)
{
    public async Task<bool> UpdateScheduleCache()
    {
        logger.LogInformation("Starting UpdateScheduleCache");
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

        bool isSuccessful = true;
        IReadOnlyList<string> groups = cacheService.GetGroups();
        foreach (var group in groups)
        {
            GetScheduleResult? scheduleResult = await elJurApi.GetSchedule(group, tomorrow.ToString("yyyyMMdd"), "yes");
            if (scheduleResult != null)
            {
                var schedule = new Schedule()
                {
                    Day = tomorrow,
                    ScheduleItems = scheduleResult.ScheduleItems
                };
                cacheService.UpdateSchedule(group, schedule);
            }
            else
            {
                isSuccessful = false;
            }
            Thread.Sleep(100);
        }
        logger.LogInformation("UpdateScheduleCache completed");
        return isSuccessful;
    }
    
    public async Task<bool> UpdateGroupsCache()
    {
        logger.LogInformation("Starting UpdateGroupsCache");
        GetGroupsResult? groupsResult = await elJurApi.GetGroups();
        if (groupsResult != null)
        {
            cacheService.UpdateGroups(groupsResult.Groups);
            logger.LogInformation("UpdateGroupsCache completed");
            return true;
        }
        return false;
    }
}