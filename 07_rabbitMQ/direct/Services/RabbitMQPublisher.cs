using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace direct.Services;

public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}

public sealed class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly IRabbitMQConnection _rmq;
    private readonly RabbitMqOptions _opt;

    public RabbitMQPublisher(IRabbitMQConnection rmq, IOptions<RabbitMqOptions> opt)
    {
        _rmq = rmq;
        _opt = opt.Value;
    }

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        // create chanel for each request
        using var chanel = await _rmq.Connection.CreateChannelAsync();

        // declare Exchange
        await chanel.ExchangeDeclareAsync(
            exchange: _opt.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        // convert message to JSON
        var json = JsonSerializer.Serialize(message);
        Console.WriteLine(json);

        // convert JSON to byte array (byte[])
        ReadOnlyMemory<Byte> body = Encoding.UTF8.GetBytes(json);

        // create properties for message
        var props = new BasicProperties
        {
            ContentType = "application/json",

            // mark message as Persistent (ghi xuống disk)
            DeliveryMode = DeliveryModes.Persistent
        };

        // publish message to Exchange
        await chanel.BasicPublishAsync(
            exchange: _opt.Exchange,
            routingKey: routingKey,
            body: body,
            basicProperties: props,
            mandatory: false // ko bắt buộc phải có queue nhận
        );
    }
}
