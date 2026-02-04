using System.Text;
using _05_authentication.Models;
using _05_authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    app.UseSwaggerUI();
    app.UseSwagger();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
