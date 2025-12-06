namespace EJournalTelegramBot.Service;

public class StartupCacheService(UpdateCacheService updateCacheService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await updateCacheService.UpdateCache();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}