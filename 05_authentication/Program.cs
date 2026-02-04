using System.Text;
using _05_authentication.Models;
using _05_authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDB"));
});
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // đọc secret key từ appsetting
    var key = builder.Configuration["Jwt:Key"];

    // cấu hình kiểm tra Token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // bắt kiểm tra Issuer (ai phát hành)
        ValidateIssuer = true,

        // bắt kiểm tra Audience (đối tượng tiêu dùng)
        ValidateAudience = true,

        // kiểm tra thời gian sống của token
        ValidateLifetime = true,

        // kiểm tra chữ ký token
        ValidateIssuerSigningKey = true,

        // giá trị issuer
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // giá trị audience
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // khóa kiểm tra chữ ký
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

        // cho phép thời gian chênh lệch server +- 30 (s)
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
