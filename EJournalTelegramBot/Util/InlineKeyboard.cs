using Telegram.Bot.Types.ReplyMarkups;

namespace EJournalTelegramBot.Util;

public static class InlineKeyboard
{
    private const int PageSize = 12;
    
    public static InlineKeyboardMarkup BuildPagedGrid(IReadOnlyList<string> items, int offset, string suffix)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();

        var pageItems = items.Skip(offset).Take(PageSize).ToList();
        
        int column = 0;
        foreach (var item in pageItems)
        {
            if (column == 2)
            {
                inline.AddNewRow();
                column = 0;
            }
            
            inline.AddButton(new InlineKeyboardButton(item, CallbackData.Select(suffix, item.Split(" ")[0])));
            column++;
        }
        
        inline.AddNewRow();
        
        if (offset > 0)
        {
            inline.AddButton("Назад", CallbackData.Page(suffix, offset-PageSize));
        }

        if (offset + PageSize < items.Count)
        {
            inline.AddButton("Дальше", CallbackData.Page(suffix, offset+PageSize));
        }
        
        return inline;
    }
    
    public static InlineKeyboardMarkup BuildGrid(IReadOnlyList<string> items, string suffix)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        
        int column = 0;
        foreach (var item in items)
        {
            if (column == 2)
            {
                inline.AddNewRow();
                column = 0;
            }
            
            inline.AddButton(new InlineKeyboardButton(item, CallbackData.Select(suffix, item.Split(" ")[0])));
            column++;
        }
        
        return inline;
    }
    
    public static InlineKeyboardMarkup BuildMainMenu()
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        inline.AddButton("Расписание по группам", CallbackData.Action(CallbackData.ChooseCourseAction));
        inline.AddNewRow();
        inline.AddButton("Расписание по преподавателям", CallbackData.Page(CallbackData.TeacherPrefix, 0));
        inline.AddNewRow();
        inline.AddButton("Расписание по кабинетам", CallbackData.Page(CallbackData.RoomPrefix, 0));
        return inline;
    }

    public static InlineKeyboardMarkup BuildChooseCourse()
    {
        return new InlineKeyboardMarkup()
            .AddButton("1 курс", CallbackData.Select(CallbackData.ChooseCoursePrefix, "1"))
            .AddButton("2 курс", CallbackData.Select(CallbackData.ChooseCoursePrefix, "2"))
            .AddNewRow()
            .AddButton("3 курс", CallbackData.Select(CallbackData.ChooseCoursePrefix, "3"))
            .AddButton("4 курс", CallbackData.Select(CallbackData.ChooseCoursePrefix, "4"));
    }
    
    public static InlineKeyboardButton BuildSubscribeButton(string prefix, string data)
    {
        return new InlineKeyboardButton("Подписаться", CallbackData.Subscribe(prefix, data));
    }
    
    public static InlineKeyboardButton BuildUnsubscribeButton(string prefix, string data)
    {
        return new InlineKeyboardButton("Отписаться", CallbackData.Unsubscribe(prefix, data));
    }
}