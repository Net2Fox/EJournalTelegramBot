namespace EJournalTelegramBot.Abstract;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken cancellationToken);
}