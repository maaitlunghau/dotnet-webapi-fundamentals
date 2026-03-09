using loyal_service.DTOs;
using LoyalService.Services;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly IRabbitMqConnection _rmq;
    private readonly RabbitMqOptions _opt;
    private readonly IServiceScopeFactory _scopeFactory;
    private IChannel? _channel;

    private static readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public RabbitMqConsumerHostedService(
        IRabbitMqConnection rmq,
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory
        )
    {
        _rmq = rmq;
        _opt = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _channel = await _rmq.Connection
            .CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: stoppingToken);
        await _channel.ExchangeDeclareAsync(
            exchange: _rmq.Exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(
            queue: _opt.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);
        foreach (var routingKey in _opt.RoutingKeys)
        {
            await _channel.QueueBindAsync(
                queue: _opt.Queue,
                exchange: _rmq.Exchange,
                routingKey: routingKey,
                cancellationToken: stoppingToken);
        }
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var bodyText = Encoding.UTF8.GetString(ea.Body.Span);
            try
            {
                var msg = JsonSerializer.Deserialize<MessageDTO>(
                    bodyText,
                    _jsonOptions)
                    ?? throw new Exception("Invalid JSON");
                Console.WriteLine(
                    $"Received Id={msg.UserId} Message={msg.Message}");
                var routingKey = ea.RoutingKey;
                switch (routingKey)
                {
                    case "user.points":
                        await HandleNewMember(msg, stoppingToken);
                        break;

                    case "order.points":
                        await HandleOrderCompleted(msg, stoppingToken);
                        break;
                }
                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    false,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await _channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    false,
                    stoppingToken);
            }
        };
        await _channel.BasicConsumeAsync(
            queue: _opt.Queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    private async Task HandleNewMember(MessageDTO msg, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var service = scope.ServiceProvider
            .GetRequiredService<LoyaltyCustomerService>();

        await service.AddPoints(
            msg.UserId,
            10,
           $"Chuc mung {msg.Message} tro thanh thanh vien moi");
    }
    private async Task HandleOrderCompleted(MessageDTO msg, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var service = scope.ServiceProvider
            .GetRequiredService<LoyaltyCustomerService>();

        await service.AddPoints(
            msg.UserId,
            20,
             $"Dat thanh cong don hang co ma {msg.Message}," +
             $" Chuc mung ban duoc cong 20 diem"
        );
    }
    public override async Task StopAsync(CancellationToken ct)
    {
        if (_channel != null && _channel.IsOpen)
            await _channel.CloseAsync(ct);

        _channel?.Dispose();

        await base.StopAsync(ct);
    }
}