using EJournalTelegramBot.Abstract;
using Telegram.Bot;

namespace EJournalTelegramBot.Service;

public class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
    : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger);