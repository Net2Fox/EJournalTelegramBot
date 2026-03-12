using Telegram.Bot;
using Telegram.Bot.Types;

namespace EJournalTelegramBot.Service;

public class StartupService(UpdateCacheService updateCacheService, ITelegramBotClient bot) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetBotCommands(cancellationToken);
        await updateCacheService.UpdateGroupsCache();
        await updateCacheService.UpdateTeacherCache();
        await updateCacheService.UpdateScheduleCache();
    }

    private async Task SetBotCommands(CancellationToken cancellationToken)
    {
        List<BotCommand> commands = 
        [
            new("group", "Получить расписание по группе"),
            new("teacher", "Получить расписание по преподавателю"),
            new("room", "Получить расписание по аудитории")
        ];

        await bot.SetMyCommands(commands, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}