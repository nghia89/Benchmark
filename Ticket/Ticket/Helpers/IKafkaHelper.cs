namespace Ticket.Helpers;

using System.Threading.Tasks;

public interface IKafkaHelper
{
    Task ProduceAsync<T>(string topic, T message);
    Task ConsumeAsync(string topic, Func<string, Task> messageHandler);
}