using System.Text;
using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Context;
using EJournalTelegramBot.Model.SQLite;
using EJournalTelegramBot.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EJournalTelegramBot.Service;

public class UpdateHandler(IOptions<AdminConfiguration> adminConfig, UpdateCacheService updateCacheService, CacheService cacheService, BotContext db, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
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
            replyMarkup: InlineKeyboard.BuildMainMenu());
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        InlineKeyboardMarkup inline = new InlineKeyboardMarkup();
        string text = "";
        
        var (prefix, action, value) = CallbackData.Parse(callbackQuery.Data!);

        switch (action)
        {
            case CallbackData.MainMenuAction:
                inline = InlineKeyboard.BuildMainMenu();
                text = "Главное меню";
                break;
            case CallbackData.ChooseCourseAction:
                inline = InlineKeyboard.BuildChooseCourse();
                text = "Выберите курс";
                break;
            
            case CallbackData.SelectAction:
                switch (prefix)
                {
                    case CallbackData.ChooseCoursePrefix:
                        inline = InlineKeyboard.BuildGrid(cacheService.GetGroups().Where(g => g[0] == Char.Parse(value)).ToList(),CallbackData.GroupPrefix);
                        text = "Выберите группу";
                        break;
                    case CallbackData.TeacherPrefix:
                        text = cacheService.GetTeacherFormattedSchedule(value);
                        break;
                    case CallbackData.RoomPrefix:
                        text = cacheService.GetRoomFormattedSchedule(value);
                        break;
                    case CallbackData.GroupPrefix:
                        text = cacheService.GetFormattedSchedule(value);
                        break;
                }
                break;
            
            case CallbackData.PageAction:
                int offset = int.Parse(value);
                switch (prefix)
                {
                    case CallbackData.GroupPrefix:
                        inline = InlineKeyboard.BuildGrid(cacheService.GetGroups().Where(g => g[0] == Char.Parse(value)).ToList(),CallbackData.GroupPrefix);
                        text = "Выберите группу";
                        break;
                    case CallbackData.TeacherPrefix:
                        inline = InlineKeyboard.BuildPagedGrid(cacheService.GetTeachers(), offset, CallbackData.TeacherPrefix);
                        text = "Выберите преподавателя";
                        break;
                    case CallbackData.RoomPrefix:
                        inline = InlineKeyboard.BuildPagedGrid(cacheService.GetRooms(), offset, CallbackData.RoomPrefix);
                        text = "Выберите аудиторию";
                        break;
                }
                break;
        }

        if (action != CallbackData.MainMenuAction)
        {
            inline.AddNewRow();
            inline.AddButton("Меню", CallbackData.Action(CallbackData.MainMenuAction));
        }
        
        await bot.EditMessageText(callbackQuery.Message!.Chat, callbackQuery.Message.MessageId, text,  ParseMode.Markdown, inline);
    }

    private async Task<bool> IsSubscribed(long chatId, string group)
    {
        return  await db.Subscriptions.AnyAsync(s => s.ChatId == chatId && s.Group == group);
    }

    private async Task SubscribeUser(long chatId, string group)
    {
        await db.Subscriptions.AddAsync(new Subscription
        {
            ChatId = chatId,
            Group = group
        });
        await db.SaveChangesAsync();
    }
    
    private async Task UnsubscribeUser(long chatId, string group)
    {
        await db.Subscriptions.Where(s => s.ChatId == chatId && s.Group == group).ExecuteDeleteAsync();

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