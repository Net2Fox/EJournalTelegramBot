using EJournalTelegramBot.Model.SQLite;
using Microsoft.EntityFrameworkCore;

namespace EJournalTelegramBot.Context;

public class BotContext : DbContext
{
    public DbSet<Subscription>  Subscriptions => Set<Subscription>();
    public BotContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=EJournalTelegramBot.db");
    }
}