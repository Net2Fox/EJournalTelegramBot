using System.Text;
using EJournalTelegramBot.Configuration;
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
        if (message.Text is not { } messageText) return;
        
        Message sentMessage = await (messageText.Split(' ')[0] switch
        {
            "/schedule" => ChooseCourse(message),
            "/teacher" => ChooseTeacher(message),
            "/room" => ChooseCourse(message),
            "/update_groups" => UpdateGroups(message),
            "/update_schedule" => UpdateSchedule(message),
            _ => Usage(message)
        });

        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message message)
    {
        List<string> lines = new()
        {
            "*Меню бота*",
            "/schedule — получить актуальное расписание"
        };
        
        if (adminConfig.Value.AdminIds != null && adminConfig.Value.AdminIds.Contains(message.Chat.Id))
        {
            lines.Add("");
            lines.Add("");
            lines.Add("У вас есть доступ к админ командам:");
            lines.Add("/update\\_groups — обновить группы");
            lines.Add("/update\\_schedule — обновить расписание");
        }
        return await bot.SendMessage(message.Chat, string.Join("\n", lines), parseMode: ParseMode.Markdown);
    }

    async Task<Message> SendSchedule(Message message)
    {
        return await bot.SendMessage(message.Chat, cacheService.GetFormattedSchedule("3ИСИП-323"), ParseMode.Markdown);
    }
    
    async Task<Message> SendScheduleByGroup(Message message,  string group)
    {
        return await bot.SendMessage(message.Chat, cacheService.GetFormattedSchedule(group), ParseMode.Markdown);
    }

    async Task<Message> ChooseCourse(Message message)
    {
        return await bot.SendMessage(message.Chat.Id, "Выберите курс",
            replyMarkup: new InlineKeyboardButton[][]
            {
                [("1 курс", "1"), ("2 курс", "2")],
                [("3 курс", "3"), ("4 курс", "4")]
                
            });
    }
    
    async Task<Message> ChooseTeacher(Message message, int offset = 0, bool update = false)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        
        int i = 0;
        int j = 0;
        
        foreach (var teacher in cacheService.GetTeachersWithOffset(offset))
        {
            if (i != 2)
            {
                inline.AddButton(new InlineKeyboardButton(teacher, teacher.Split(" ")[0]));
                i = i + 1;
                j = j + 1;
            }
            else
            {
                inline.AddNewRow();
                i = 0;
                inline.AddButton(new InlineKeyboardButton(teacher, teacher.Split(" ")[0]));
                i = i + 1;
                j = j + 1;
            }
        }
        inline.AddNewRow();
        inline.AddButton("Назад", "BackTeacher");
        inline.AddButton("Дальше", "NextTeacher");
        inline.AddNewRow();
        inline.AddButton("Меню", "Back");

        if (update)
        {
            return await bot.EditMessageReplyMarkup(message.Chat, message.MessageId, inline);
        }
        else
        {
            return await bot.SendMessage(message.Chat.Id, "Выберите преподавателя",
                replyMarkup: inline);
        }
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

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        int i = 0;
        int j = 0;
        switch (callbackQuery.Data)
        {
            case "1" or "2" or "3" or "4":
                foreach (var group in cacheService.GetGroups().Where(g => g[0] == Char.Parse(callbackQuery.Data)))
                {
                    if (i != 2)
                    {
                        inline.AddButton(new InlineKeyboardButton(group, group));
                        i = i + 1;
                        j = j + 1;
                    }
                    else
                    {
                        inline.AddNewRow();
                        i = 0;
                        inline.AddButton(new InlineKeyboardButton(group, group));
                        i = i + 1;
                        j = j + 1;
                    }
                }
                inline.AddNewRow();
                inline.AddButton("Назад", "Back");
                break;
            case "Back":
                inline = new InlineKeyboardButton[][]
                {
                    [("1 курс", "1"), ("2 курс", "2")],
                    [("3 курс", "3"), ("4 курс", "4")]
                };
                break;
            case "BackTeacher":
                await ChooseTeacher(callbackQuery.Message, -12, true);
                return;
            case "NextTeacher":
                await ChooseTeacher(callbackQuery.Message, 12, true);
                return;
            default:
                await SendScheduleByGroup(callbackQuery.Message, callbackQuery.Data);
                return;
        }
        
        await bot.EditMessageReplyMarkup(callbackQuery.Message.Chat, callbackQuery.Message.MessageId, inline);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}