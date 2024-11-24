namespace Ticket.Helpers;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

public class KafkaHelper : IKafkaHelper
{
    private readonly IProducer<Null, string> _producer;
    private readonly ConsumerConfig _consumerConfig;

    public KafkaHelper(ProducerConfig producerConfig, ConsumerConfig consumerConfig)
    {
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _consumerConfig = consumerConfig;
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

    public async Task ConsumeAsync(string topic, Func<string, Task> messageHandler)
    {
        using var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build();

        consumer.Subscribe(topic);

        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume();

                // Xử lý tin nhắn
                await messageHandler(consumeResult.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
        finally
        {
            consumer.Close();
        }
    }
}