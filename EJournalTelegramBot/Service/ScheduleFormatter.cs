using System.Text;
using EJournalTelegramBot.Model;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class ScheduleFormatter
{
    private string Format(string title, Schedule? schedule, Func<ScheduleItem, string> formatItem)
    {
        if (schedule is null)
        {
            return "Расписания нет!";
        }
        
        StringBuilder formattedSchedule = new StringBuilder();
        formattedSchedule.AppendLine($"Расписание для *{title}* на *{schedule.Day:dd.MM.yyyy}*:");
        formattedSchedule.AppendLine();

        if (schedule.ScheduleItems.Count == 0)
        {
            formattedSchedule.AppendLine("Пар нет!");
        }
        var orderedScheduleItems = schedule.ScheduleItems
            .OrderBy(item => int.Parse(item.Num))
            .ToList();
        
        for (int i = 0; i < orderedScheduleItems.Count; i++)
        {
            ScheduleItem scheduleItem =  orderedScheduleItems[i];
            DateTime start = DateTime.Parse(scheduleItem.StartTime);
            DateTime end = DateTime.Parse(scheduleItem.EndTime);
            
            if (i != 0 && int.Parse(scheduleItem.Num) != 1)
            {
                int rest = (TimeSpan.Parse(scheduleItem.StartTime) - 
                            TimeSpan.Parse(orderedScheduleItems[i - 1].EndTime)).Minutes;
                formattedSchedule.AppendLine($"_Перерыв {rest} минут_\n");
            }
            
            formattedSchedule.Append($"{scheduleItem.Num}.* {start:HH:mm}-{end:HH:mm}*. ");
            formattedSchedule.AppendLine(formatItem(scheduleItem));
        }
        
        return formattedSchedule.ToString();
    }

    public string FormatGroup(string group, Schedule? schedule)
    {
        return Format(group, schedule, item => $"_{item.Teacher}_ — {Room(item.Room)}");
    }
    
    public string FormatTeacher(string teacher, Schedule? schedule)
    {
        return Format(teacher, schedule, item => $"_{item.SubGroup}_ — {Room(item.Room)}");
    }
    
    public string FormatRoom(string room, Schedule? schedule)
    {
        return Format(room, schedule, item => $"_{item.Teacher}_ — {item.SubGroup}");
    }
    
    private string Room(string room)
    {
        return room switch
        {
            "ВМ" => "Выездное мероприятие",
            "ДОТ" => "ДОТ",
            _ => $"ауд. {room}"
        };
    }
}