using System.Collections.Concurrent;
using EJournalTelegramBot.Model;

namespace EJournalTelegramBot.Service;

public class CacheService(ScheduleFormatter formatter)
{
    private readonly ConcurrentDictionary<string, Schedule> _cache = new();

    public Schedule? GetSchedule(string groupName)
    {
        return _cache.GetValueOrDefault(groupName);
    }
    
    public string GetFormattedSchedule(string groupName)
    {
        return formatter.Format(groupName, _cache.GetValueOrDefault(groupName));
    }

    public void UpdateSchedule(string groupName, Schedule schedule)
    {
        _cache[groupName] = schedule;
    }

    public IReadOnlyDictionary<string, Schedule> GetSchedules()
    {
        return new  Dictionary<string, Schedule>(_cache);
    }
}