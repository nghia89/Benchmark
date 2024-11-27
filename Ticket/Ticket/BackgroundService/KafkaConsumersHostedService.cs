namespace Ticket.BackgroundService;

public class KafkaConsumersHostedService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IEnumerable<IKafkaConsumerService> _consumerServices;

    public KafkaConsumersHostedService(IEnumerable<IKafkaConsumerService> consumerServices)
    {
        _consumerServices = consumerServices;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("KafkaConsumerBackgroundService started.");

        // Khởi chạy tất cả các consumer
        var consumingTasks = _consumerServices
            .Select(service => Task.Run(() => service.StartConsumingAsync(stoppingToken), stoppingToken))
            .ToArray();

        try
        {
            await Task.WhenAll(consumingTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in consuming tasks: {ex.Message}");
        }

        Console.WriteLine("KafkaConsumerBackgroundService stopped.");
    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping KafkaConsumerBackgroundService...");
        return base.StopAsync(cancellationToken);
    }
}
