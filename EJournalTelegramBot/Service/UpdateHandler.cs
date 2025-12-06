using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EJournalTelegramBot.Service;

public class UpdateHandler(CacheService cacheService, ScheduleFormatter formatter, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
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
            "/schedule" => SendSchedule(message),
            _ => Usage(message)
        });
        
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message message)
    {
        const string usage = """
                             *Меню бота*
                             /schedule - получить расписание
                             """;
        return await bot.SendMessage(message.Chat, usage, parseMode: ParseMode.Markdown);
    }

    async Task<Message> SendSchedule(Message message)
    {
        return await bot.SendMessage(message.Chat, cacheService.GetFormattedSchedule("3ИСИП-323"), ParseMode.Markdown);
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}