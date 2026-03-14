using System.Text;
using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Util;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EJournalTelegramBot.Service;

public class UpdateHandler(IOptions<AdminConfiguration> adminConfig, UpdateCacheService updateCacheService, CacheService cacheService, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handle Error: {Exception}", exception);
        if (exception is RequestException)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message message)
    {
        logger.LogInformation("Received message type: {MessageType}", message.Type);
        
        await bot.SendMessage(message.Chat.Id, "Главное меню",
            replyMarkup: MainMenu());
    }

    private InlineKeyboardMarkup MainMenu()
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        inline.AddButton("Расписание по группам", "Group");
        inline.AddNewRow();
        inline.AddButton("Расписание по преподавателям", "Teacher");
        inline.AddNewRow();
        inline.AddButton("Расписание по кабинетам", "Room");
        return inline;
    }

    private InlineKeyboardMarkup ChooseCourse()
    {
        return new InlineKeyboardMarkup()
            .AddButton("1 курс", "1")
            .AddButton("2 курс", "2")
            .AddNewRow()
            .AddButton("3 курс", "3")
            .AddButton("4 курс", "4");
    }

    private InlineKeyboardMarkup BuildGridInlineKeyboard(IReadOnlyList<string> items, string suffix)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        
        int i = 0;
        foreach (var item in items)
        {
            if (i != 2)
            {
                inline.AddButton(new InlineKeyboardButton(item, $"{item.Split(" ")[0]} {suffix}"));
                i++;
            }
            else
            {
                inline.AddNewRow();
                i = 0;
                inline.AddButton(new InlineKeyboardButton(item, $"{item.Split(" ")[0]} {suffix}"));
                i++;
            }
        }
        inline.AddNewRow();
        inline.AddButton("Назад", $"Back{suffix}");
        inline.AddButton("Дальше", $"Next{suffix}");
        return inline;
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        string text = "";
        
        switch (callbackQuery.Data)
        {
            case "Back":
                inline = MainMenu();
                text = "Главное меню";
                break;
            case "1" or "2" or "3" or "4":
                inline = BuildGridInlineKeyboard(cacheService.GetGroups().Where(g => g[0] == Char.Parse(callbackQuery.Data)).ToList(), CallbackData.GroupSuffix);
                text = "Выберите группу";
                break;
            case $"Back{CallbackData.TeacherSuffix}":
                inline = BuildGridInlineKeyboard(cacheService.GetTeachers(-12), CallbackData.TeacherSuffix);
                text = "Выберите преподавателя";
                break;
            case $"Next{CallbackData.TeacherSuffix}":
                inline = BuildGridInlineKeyboard(cacheService.GetTeachers(12), CallbackData.TeacherSuffix);
                text = "Выберите преподавателя";
                break;
            case $"Back{CallbackData.RoomSuffix}":
                inline = BuildGridInlineKeyboard(cacheService.GetRooms(-12), CallbackData.RoomSuffix);
                text = "Выберите аудиторию";
                break;
            case $"Next{CallbackData.RoomSuffix}":
                inline = BuildGridInlineKeyboard(cacheService.GetRooms(12), CallbackData.RoomSuffix);
                text = "Выберите аудиторию";
                break;
            case "Teacher":
                inline = BuildGridInlineKeyboard(cacheService.GetTeachers(), CallbackData.TeacherSuffix);
                text = "Выберите преподавателя";
                break;
            case "Group":
                inline = ChooseCourse();
                text = "Выберите курс";
                break;
            case "Room":
                inline = BuildGridInlineKeyboard(cacheService.GetRooms(), CallbackData.RoomSuffix);
                text = "Выберите аудиторию";
                break;
            case string t when t.Contains("TCH"):
                text = cacheService.GetTeacherFormattedSchedule(callbackQuery.Data.Split(" ")[0]);
                break;
            case string t when t.Contains("RM"):
                text = cacheService.GetRoomFormattedSchedule(callbackQuery.Data.Split(" ")[0]);
                break;
            case string t when t.Contains("GRP"):
                text = cacheService.GetFormattedSchedule(callbackQuery.Data.Split(" ")[0]);
                break;
        }
        inline.AddNewRow();
        inline.AddButton("Меню", "Back");
        await bot.EditMessageText(callbackQuery.Message.Chat, callbackQuery.Message.MessageId, text,  ParseMode.Markdown);
        await bot.EditMessageReplyMarkup(callbackQuery.Message.Chat, callbackQuery.Message.MessageId, inline);
    }
    
    async Task<Message> UpdateSchedule(Message message)
    {
        if (adminConfig.Value.AdminIds != null && adminConfig.Value.AdminIds.Contains(message.Chat.Id))
        {
            if (await updateCacheService.UpdateScheduleCache())
            {
                return await bot.SendMessage(message.Chat, "Расписание обновлено!", ParseMode.Markdown);
            }
        
            return await bot.SendMessage(message.Chat, "Произошла ошибка при обновлении расписания!", ParseMode.Markdown);
        }
        
        return await bot.SendMessage(message.Chat, "Нет доступа!", ParseMode.Markdown);
    }
    
    async Task<Message> UpdateGroups(Message message)
    {
        if (adminConfig.Value.AdminIds != null && adminConfig.Value.AdminIds.Contains(message.Chat.Id))
        {
            if (await updateCacheService.UpdateGroupsCache())
            {
                return await bot.SendMessage(message.Chat, "Список групп обновлён!", ParseMode.Markdown);
            }

            return await bot.SendMessage(message.Chat, "Произошла ошибка при обновлении списка групп!", ParseMode.Markdown);
        }

        return await bot.SendMessage(message.Chat, "Нет доступа!", ParseMode.Markdown);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}