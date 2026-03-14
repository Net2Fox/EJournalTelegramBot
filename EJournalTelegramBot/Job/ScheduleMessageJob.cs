using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Service;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EJournalTelegramBot.Job;

public class ScheduleMessageJob(CacheService cacheService, UpdateCacheService updateCacheService, IOptionsMonitor<BroadcastConfiguration> broadcast, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IJob
{
    public static readonly JobKey Key = new("ScheduleMessageJob");
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Execute {KeyName}", Key.Name);
        await UpdateSchedule();
        await BroadcastSchedule();
    }

    private async Task UpdateSchedule()
    {
        await updateCacheService.UpdateScheduleCache();
    }

    private async Task BroadcastSchedule()
    {
        var broadcastEntities = broadcast.CurrentValue.BroadcastEntities;
        if (broadcastEntities == null)
        {
            return;
        }
        
        foreach (var broadcastEntity in broadcastEntities)
        {
            if (broadcastEntity.Subscriptions != null)
            {
                foreach (string subscription in broadcastEntity.Subscriptions)
                {
                    await bot.SendMessage(broadcastEntity.ChatId, cacheService.GetFormattedSchedule(subscription), ParseMode.Markdown);
                }
            }
        }
    }
}