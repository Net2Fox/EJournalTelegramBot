using System.Text;
using EJournalTelegramBot.Model;
using EJournalTelegramBot.Model.ElJurApi;

namespace EJournalTelegramBot.Service;

public class ScheduleFormatter
{
    public string Format(string groupName, Schedule? schedule)
    {
        if (schedule != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Расписание для *{groupName}* на  *{schedule.Day:dd.MM.yyyy}*:");
            sb.AppendLine();
            if (schedule.ScheduleItems != null &&  schedule.ScheduleItems.Count != 0)
            {
                for (int i = 0; i < schedule.ScheduleItems.Count; i++)
                {
                    ScheduleItem sc =  schedule.ScheduleItems[i];
                    DateTime start = DateTime.Parse(sc.StartTime);
                    DateTime end = DateTime.Parse(sc.EndTime);
                    if (i != 0 && int.Parse(sc.Num)!= 1)
                    {
                        int rest = (TimeSpan.Parse(sc.StartTime) - 
                                    TimeSpan.Parse(schedule.ScheduleItems[i - 1].EndTime)).Minutes;
                        sb.AppendLine($"_Перерыв {rest} минут_\n");
                    }
                    sb.Append($"{sc.Num}.* {start:HH:mm}-{end:HH:mm}*. _{sc.Teacher}_ — ");
                    sb.AppendLine(sc.Room == "ВМ" ? "Выездное мероприятие" : sc.Room == "ДОТ" ? "ДОТ" : $"ауд. {sc.Room}");
                }
            }
            else
            {
                sb.AppendLine("Пар нет!");
            }
            return sb.ToString();
        }
        else
        {
            return "Расписания нет!";
        }
    }
}