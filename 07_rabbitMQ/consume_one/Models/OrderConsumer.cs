
using consume_one.Services;
using direct.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace consume_one.Services;

public record OrderCreated(
    string OrderId,
    double Price);
public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    // Inject connection dùng chung (Singleton)
    private readonly IRabbitMqConnection _rmq;

    // Lưu cấu hình RabbitMQ (Queue name...)
    private readonly RabbitMqOptions _opt;

    // Channel dùng để consume (channel KHÔNG thread-safe)
    private IChannel? _channel;

    // Cấu hình JSON deserialize (không phân biệt hoa thường)
    private static readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    // Constructor nhận dependency từ DI container
    public RabbitMqConsumerHostedService(
        IRabbitMqConnection rmq, // Connection dùng chung
                                 // Config từ appsettings
        IOptions<RabbitMqOptions> options)

    {
        _rmq = rmq; // Gán connection
        _opt = options.Value; // Lấy config thực tế
    }

    // Hàm chính chạy vòng đời của BackgroundService
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Tạo channel từ connection dùng chung
        _channel = await _rmq.Connection
            .CreateChannelAsync(cancellationToken: stoppingToken);

        // QoS = giới hạn số message chưa ACK cùng lúc
        // prefetchCount = 10 nghĩa là tối đa
        // 10 message đang xử lý song song
        await _channel.BasicQosAsync(
            prefetchSize: 0,      // Không giới hạn theo size
            prefetchCount: 10,    // Tối đa 10 message chưa ACK
            global: false,        // Áp dụng cho channel này
            cancellationToken: stoppingToken);

        // Khai báo Fanout Exchange
        // Idempotent: gọi nhiều lần không lỗi
        await _channel.ExchangeDeclareAsync(
            exchange: _rmq.Exchange, // Lấy exchange từ connection
            type: ExchangeType.Direct, // Direct → broadcast
            durable: true,           // Không mất khi restart server
            autoDelete: false,       // Không tự xóa
            cancellationToken: stoppingToken);

        // Khai báo Queue
        await _channel.QueueDeclareAsync(
            queue: _opt.Queue, // Tên queue từ config
            durable: true,     // Không mất khi restart
            exclusive: false,  // Cho phép nhiều consumer
            autoDelete: false, // Không tự xóa
            cancellationToken: stoppingToken);

        // Bind Queue vào Exchange
        await _channel.QueueBindAsync(
            queue: _opt.Queue,
            exchange: _rmq.Exchange,
            routingKey: _opt.RoutingKey,
            cancellationToken: stoppingToken);

        // Tạo consumer async
        var consumer = new AsyncEventingBasicConsumer(_channel);

        // Đăng ký event khi có message tới
        consumer.ReceivedAsync += async (_, ea) =>
        {
            // Chuyển byte[] → string
            var bodyText = Encoding.UTF8.GetString(ea.Body.Span);
            try
            {
                // Deserialize JSON thành object OrderCreated
                var msg = JsonSerializer.Deserialize<OrderCreated>(
                    bodyText, _jsonOptions)
                    ?? throw new Exception("Invalid JSON message.");
                Console.WriteLine(
                    $"Received OrderId=${msg.OrderId}, Price={msg.Price}");

                // Gọi xử lý nghiệp vụ
                await HandleMessageAsync(msg, stoppingToken);

                // ACK = báo RabbitMQ xóa message khỏi queue
                await _channel.BasicAckAsync(
                    ea.DeliveryTag, // ID message
                    multiple: false, // Chỉ ACK message này
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // NACK = báo xử lý lỗi
                // requeue: false để không lặp vô hạn
                await _channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: stoppingToken);
            }
        };

        // Bắt đầu consume
        // autoAck = false → phải ACK thủ công
        await _channel.BasicConsumeAsync(
            queue: _opt.Queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        // Giữ service chạy vô hạn đến khi bị stop
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    // Hàm xử lý nghiệp vụ thực tế
    private static Task HandleMessageAsync(
        OrderCreated msg,
        CancellationToken ct)
    {
        // TODO: Thêm xử lý thật như ghi DB, gọi API...
        return Task.CompletedTask;
    }

    // Hàm chạy khi service dừng
    public override async Task StopAsync(CancellationToken ct)
    {
        // Nếu channel còn mở thì đóng lại
        if (_channel != null && _channel.IsOpen)
            await _channel.CloseAsync(ct);

        // Giải phóng tài nguyên
        _channel?.Dispose();

        // Gọi base StopAsync
        await base.StopAsync(ct);
    }
}