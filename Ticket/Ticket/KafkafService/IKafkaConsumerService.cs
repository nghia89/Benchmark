namespace Ticket.BackgroundService;

public interface IKafkaConsumerService
{
    Task StartConsumingAsync(CancellationToken stoppingToken);
}
