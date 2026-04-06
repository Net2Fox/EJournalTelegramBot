using EJournalTelegramBot.Context;
using EJournalTelegramBot.Service;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EJournalTelegramBot.Job;

public class ScheduleMessageJob(CacheService cacheService, UpdateCacheService updateCacheService, BotContext db, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IJob
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
        var subscriptions = await db.Subscriptions.ToListAsync();

        foreach (var subscription in subscriptions)
        {
            await bot.SendMessage(subscription.ChatId, cacheService.GetFormattedSchedule(subscription.Prefix, subscription.Value), ParseMode.Markdown);
        }
    }
}