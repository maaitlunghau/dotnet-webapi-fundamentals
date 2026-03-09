namespace direct.Controllers
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Exchange { get; set; } = default!;
    }
}
