using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace user_service.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(string routingKey, T message);
    }
    public sealed class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IRabbitMqConnection _rmq;
        private readonly RabbitMqOptions _opt;
        public RabbitMqPublisher(IRabbitMqConnection rmq, IOptions<RabbitMqOptions> opt)
        {
            _rmq = rmq;
            _opt = opt.Value;
        }
        public async Task PublishAsync<T>(string routingKey, T message)
        {
            //tao chanel cho moi request
            using var chanel = await _rmq.Connection.CreateChannelAsync();
            // Khai bao Exchange
            await chanel.ExchangeDeclareAsync(
                exchange: _opt.Exchange,
                type: ExchangeType.Direct,
                 durable: true,
                 // Doi server xac nhan
                 noWait: false,
                 autoDelete: false,
                 arguments: null
                );
            //chuyen message thanh json
            var json = JsonSerializer.Serialize(message);
            Console.WriteLine(json);
            //chuyen thanh byte[]
            ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(json);
            // tao thuoc tinh cho message
            var props = new BasicProperties
            {
                ContentType = "application/json",
                //Danh dau message la Persistent(ghi xuong disk)
                DeliveryMode = DeliveryModes.Persistent
            };
            //publish message len Exchange voi routingKey
            await chanel.BasicPublishAsync(
                exchange: _opt.Exchange,
                routingKey: routingKey,
                body: body,
                basicProperties: props,
                // khong bat buoc phai co queue nhan
                mandatory: false
                );

        }
    }
}
