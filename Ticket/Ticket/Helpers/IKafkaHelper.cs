namespace Ticket.Helpers;

using System.Threading.Tasks;

public interface IKafkaHelper
{
    Task ProduceAsync<T>(string topic, T message);
    Task ConsumeAsync(string topic,string groupId, Func<string, Task> messageHandler, CancellationToken stoppingToken);
}