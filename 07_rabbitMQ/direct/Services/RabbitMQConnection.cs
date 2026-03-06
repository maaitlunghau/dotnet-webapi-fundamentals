using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace direct.Services;

public interface IRabbitMQConnection : IAsyncDisposable
{
    IConnection Connection { get; }
    string Exchange { get; }
}

// sealed: ??
public sealed class RabbitMQConnection : IRabbitMQConnection
{
    public IConnection Connection { get; }

    public string Exchange { get; }

    public RabbitMQConnection(IOptions<RabbitMqOptions> options)
    {
        var opt = options?.Value ?? throw new ArgumentException();
        Exchange = opt.Exchange;

        var factory = new ConnectionFactory
        {
            HostName = opt.HostName,
            UserName = opt.UserName,
            Password = opt.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        // create connection
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
