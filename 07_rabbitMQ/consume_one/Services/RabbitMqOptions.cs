namespace consume_one.Services;

public class RabbitMqOptions
{
    public string HostName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Exchange { get; set; } = default!;
    public string Queue { get; set; } = default!;
    public string RoutingKey { get; set; } = default!;
}
