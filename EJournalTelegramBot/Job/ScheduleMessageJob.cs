using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Service;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EJournalTelegramBot.Job;

public class ScheduleMessageJob(CacheService cacheService, UpdateCacheService updateCacheService, IOptions<ScheduleConfiguration> config, ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IJob
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
        await updateCacheService.UpdateCache();
    }

    private async Task BroadcastSchedule()
    {
        List<long>? chatIds = config.Value.ChatIds;
        if (chatIds != null && chatIds.Count != 0)
        {
            foreach (long chatId in chatIds)
            {
                await bot.SendMessage(chatId, cacheService.GetFormattedSchedule("3ИСИП-323"), ParseMode.Markdown);
            }
        }
    }
}