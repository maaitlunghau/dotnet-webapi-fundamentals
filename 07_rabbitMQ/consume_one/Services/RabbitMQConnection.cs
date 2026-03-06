using consume_one.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace direct.Services
{
    public interface IRabbitMqConnection : IAsyncDisposable
    {
        IConnection Connection { get; }
        string Exchange { get; }
    }
    public sealed class RabbitMqConnection : IRabbitMqConnection
    {
        public IConnection Connection { get; }

        public string Exchange { get; }
        public RabbitMqConnection(IOptions<RabbitMqOptions> options)
        {
            var opt = options?.Value ?? throw new ArgumentNullException();
            Exchange = opt.Exchange;
            var factory = new ConnectionFactory
            {
                HostName = opt.HostName,
                UserName = opt.UserName,
                Password = opt.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            //Tao ket noi toi RabbitMq
            Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

        }

        public async ValueTask DisposeAsync()
        {
            if (Connection != null)
            {
                if (Connection.IsOpen)
                {
                    await Connection.CloseAsync();
                    Connection.Dispose();
                }
            }
        }
    }
}
