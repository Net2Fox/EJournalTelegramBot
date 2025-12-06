using EJournalTelegramBot.Abstract;

namespace EJournalTelegramBot.Service;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);