using Telegram.Bot;
using Telegram.Bot.Types;

namespace EJournalTelegramBot.Service;

public class StartupService(UpdateCacheService updateCacheService, ITelegramBotClient bot) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetBotCommands(cancellationToken);
        await updateCacheService.UpdateGroupsCache();
        await updateCacheService.UpdateScheduleCache();
    }

    private async Task SetBotCommands(CancellationToken cancellationToken)
    {
        List<BotCommand> commands = 
        [
            new("menu", "Главное меню"),
        ];

        await bot.SetMyCommands(commands, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}