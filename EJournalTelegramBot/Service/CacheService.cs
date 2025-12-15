using System.Collections.Concurrent;
using EJournalTelegramBot.Model;

namespace EJournalTelegramBot.Service;

public class CacheService(ScheduleFormatter formatter)
{
    private readonly ConcurrentDictionary<string, Schedule> _scheduleCache = new();
    private readonly List<string> _groupCache = new();

    public Schedule? GetSchedule(string groupName)
    {
        return _scheduleCache.GetValueOrDefault(groupName);
    }
    
    public string GetFormattedSchedule(string groupName)
    {
        return formatter.Format(groupName, _scheduleCache.GetValueOrDefault(groupName));
    }

    public void UpdateSchedule(string groupName, Schedule schedule)
    {
        _scheduleCache[groupName] = schedule;
    }

    public void UpdateGroups(List<string> groups)
    {
        _groupCache.Clear();
        _groupCache.AddRange(groups);
    }

    public IReadOnlyDictionary<string, Schedule> GetSchedules()
    {
        return new Dictionary<string, Schedule>(_scheduleCache);
    }

    public IReadOnlyList<string> GetGroups()
    {
        return new List<string>(_groupCache);
    }
}