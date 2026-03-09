using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace direct.Controllers;

public interface IRabbitMQConnection : IAsyncDisposable
{
    IConnection Connection { get; }
    string Exchange { get; }
}

public sealed class RabbitMQConnection : IRabbitMQConnection
{
    public IConnection Connection { get; }
    public string Exchange { get; }

    public RabbitMQConnection(IOptions<RabbitMQOptions> options)
    {
        var opt = options?.Value ?? throw new ArgumentNullException(nameof(options));
        Exchange = opt.Exchange;

        var factory = new ConnectionFactory
        {
            HostName = opt.HostName,
            UserName = opt.UserName,
            Password = opt.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (Connection is null) return;

        if (Connection.IsOpen)
        {
            await Connection.CloseAsync();
            Connection.Dispose();
        }
    }
}