using EJournalTelegramBot;
using EJournalTelegramBot.Configuration;
using EJournalTelegramBot.Job;
using EJournalTelegramBot.Service;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<CacheService>();
        services.AddSingleton<UpdateCacheService>();
        services.AddSingleton<ScheduleFormatter>();
        
        services.AddTransient<ScheduleMessageJob>();

        services.Configure<ScheduleConfiguration>(context.Configuration.GetSection("ScheduleConfiguration"));
        services.Configure<QuartzOptions>(context.Configuration.GetSection("Quartz"));
        services.AddQuartz(q =>
        {
            
            q.SchedulerId = "QuartzScheduler";

            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 3; });

            q.ScheduleJob<ScheduleMessageJob>(trigger => trigger
                .WithIdentity("ScheduleMessageJob Trigger")
                .ForJob(ScheduleMessageJob.Key)
                .WithCronSchedule("0 0 16 ? * MON,TUE,WED,THU,FRI *", x => x
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow")))
            );
        });
            
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));
        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
            .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
            {
                BotConfiguration? botConfiguration = serviceProvider.GetService<IOptions<BotConfiguration>>()?.Value;
                ArgumentNullException.ThrowIfNull(botConfiguration);
                TelegramBotClientOptions options = new(botConfiguration.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

        services.Configure<ElJurApiConfiguration>(context.Configuration.GetSection("ElJurApiConfiguration"));
        services.AddHttpClient("ejournal_client").RemoveAllLoggers()
            .AddTypedClient<ElJurApiService>((httpClient, serviceProvider) =>
            {
                ElJurApiConfiguration? elJurApiConfiguration =
                    serviceProvider.GetService<IOptions<ElJurApiConfiguration>>()?.Value;
                ArgumentNullException.ThrowIfNull(elJurApiConfiguration);
                ElJurApiOptions options = new(elJurApiConfiguration.BaseUrl, elJurApiConfiguration.DevKey,
                    elJurApiConfiguration.AuthToken, elJurApiConfiguration.Vendor);
                
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                
                return new ElJurApiService(options, httpClient);
            });

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
        services.AddHostedService<StartupCacheService>();
    })
    .Build();
    
await host.RunAsync();