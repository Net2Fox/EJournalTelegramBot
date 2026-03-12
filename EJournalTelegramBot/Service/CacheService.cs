using System.Collections.Concurrent;
using EJournalTelegramBot.Model;

namespace EJournalTelegramBot.Service;

public class CacheService(ScheduleFormatter formatter)
{
    private readonly ConcurrentDictionary<string, Schedule> _scheduleCache = new();
    private readonly List<string> _groupCache = new();

    private readonly List<string> _teacherCache = new();
    private int _offset = 0;
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

    public void UpdateTeachers(List<string> teachers)
    {
        _teacherCache.Clear();
        _teacherCache.AddRange(teachers);
    }
    public IReadOnlyDictionary<string, Schedule> GetSchedules()
    {
        return new Dictionary<string, Schedule>(_scheduleCache);
    }

    public IReadOnlyList<string> GetGroups()
    {
        return new List<string>(_groupCache);
    }
    public IReadOnlyList<string> GetTeachers()
    {
        return new List<string>(_teacherCache);
    }
    
    public IReadOnlyList<string> GetTeachersWithOffset(int offset)
    {
        _offset += offset;
        return new List<string>(_teacherCache[_offset..(_offset + 12)]);
    }
}