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
    
    private int _offsetTeacher = 0;
    private int _offsetRoom = 0;
    
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
        return new List<string>(_groupCache);
    }
    
    public IReadOnlyList<string> GetTeachers()
    {
        return new List<string>(_teacherScheduleCache.Keys.OrderBy(t => t).ToList()[_offsetTeacher..(_offsetTeacher + 12)]);
    }
    
    public IReadOnlyList<string> GetTeachers(int offset)
    {
        _offsetTeacher += offset;
        return new List<string>(_teacherScheduleCache.Keys.OrderBy(t => t).ToList()[_offsetTeacher..(_offsetTeacher + 12)]);
    }
    
    public IReadOnlyList<string> GetRooms()
    {
        return new List<string>(_roomScheduleCache.Keys.OrderBy(r => r).ToList()[_offsetTeacher..(_offsetTeacher + 12)]);
    }
    
    public IReadOnlyList<string> GetRooms(int offset)
    {
        _offsetRoom += offset;
        return new List<string>(_roomScheduleCache.Keys.OrderBy(r => r).ToList()[_offsetRoom..(_offsetRoom + 12)]);
    }
}