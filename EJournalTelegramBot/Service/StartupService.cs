using Telegram.Bot;
using Telegram.Bot.Types;

namespace EJournalTelegramBot.Service;

public class StartupService(UpdateCacheService updateCacheService, ITelegramBotClient bot) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetBotCommands(cancellationToken);
        await updateCacheService.UpdateCache();
    }

    private async Task SetBotCommands(CancellationToken cancellationToken)
    {
        List<BotCommand> commands = 
        [
            new("schedule", "Получить актуальное расписание")
        ];

        await bot.SetMyCommands(commands, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}