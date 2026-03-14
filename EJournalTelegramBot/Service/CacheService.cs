using System.Collections.Concurrent;
using EJournalTelegramBot.Model;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class CacheService(ScheduleFormatter formatter)
{
    private readonly ConcurrentDictionary<string, Schedule> _scheduleCache = new();
    private readonly ConcurrentDictionary<string, Schedule> _teacherScheduleCache = new();
    private readonly ConcurrentDictionary<string, Schedule> _roomScheduleCache = new();
    
    private readonly List<string> _groupCache = new();
    
    public Schedule? GetSchedule(string groupName)
    {
        return _scheduleCache.GetValueOrDefault(groupName);
    }
    
    public string GetFormattedSchedule(string groupName)
    {
        return formatter.FormatGroup(groupName, _scheduleCache.GetValueOrDefault(groupName));
    }
    
    public string GetTeacherFormattedSchedule(string teacher)
    {
        return formatter.FormatTeacher(teacher, _teacherScheduleCache.GetValueOrDefault(teacher));
    }
    
    public string GetRoomFormattedSchedule(string room)
    {
        return formatter.FormatRoom(room, _roomScheduleCache.GetValueOrDefault(room));
    }

    public void UpdateSchedule(string groupName, Schedule schedule)
    {
        _scheduleCache[groupName] = schedule;
    }
    
    public void AddTeacherSchedule(string teacher, ScheduleItem schedule, DateTime date)
    {
        _teacherScheduleCache.TryAdd(teacher, new Schedule { Day = date, ScheduleItems = new List<ScheduleItem>()});
        _teacherScheduleCache[teacher].ScheduleItems.Add(schedule);
    }
    
    public void AddRoomSchedule(string room, ScheduleItem schedule, DateTime date)
    {
        _roomScheduleCache.TryAdd(room, new Schedule { Day = date, ScheduleItems = new List<ScheduleItem>() });
        _roomScheduleCache[room].ScheduleItems.Add(schedule);
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
        return new List<string>(_groupCache.OrderBy(g => g));
    }
    
    public IReadOnlyList<string> GetTeachers()
    {
        return new List<string>(_teacherScheduleCache.Keys.OrderBy(t => t));
    }
    
    public IReadOnlyList<string> GetRooms()
    {
        return new List<string>(_roomScheduleCache.Keys.OrderBy(r => r));
    }
}