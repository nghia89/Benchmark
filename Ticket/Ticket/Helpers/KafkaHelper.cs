namespace Ticket.Helpers;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

public class KafkaHelper : IKafkaHelper
{
    private readonly IProducer<Null, string> _producer;

    public KafkaHelper(ProducerConfig producerConfig)
    {
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        var serializedMessage = JsonSerializer.Serialize(message);

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedMessage });
            Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> ex)
        {
            Console.WriteLine($"Error producing message: {ex.Error.Reason}");
        }
    }
    public async Task ConsumeAsync(string topic,string groupId, Func<string, Task> handleMessage, CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Consume message
                    var result = consumer.Consume(stoppingToken);

                    if (result != null)
                    {
                        // Handle message
                        await handleMessage(result.Message.Value);

                        // Commit offset sau khi xử lý thành công
                        consumer.Commit(result);
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Consume error: {ex.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Consumer operation canceled.");
                    break;
                }
            }
        }
        finally
        {
            consumer.Close(); // Đảm bảo consumer được đóng
        }
    }
}