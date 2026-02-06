using System.IdentityModel.Tokens.Jwt;
using System.Text;
using backend.Data;
using backend.Repository;
using backend.Service;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// configure JSON serialization to handle circular references
// nếu ko muốn dùng IgnoreCycles thì dùng DTO để tránh lỗi (ReferenceHandler.IgnoreCycles)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDB"));
});

// register DI container for TokenService
builder.Services.AddSingleton<TokenService>();

// register DI container for UserService
builder.Services.AddScoped<IUserRepository, UserService>();

// register DI container for OtpService
builder.Services.AddScoped<OtpService>();

// configure authentication middleware
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // read info setting from appsetting.json
    var secretKey = builder.Configuration["JWT:Key"]
        ?? throw new ArgumentException("JWT:Key is not configured properly.");
    var issuer = builder.Configuration["JWT:Issuer"]
        ?? throw new ArgumentException("JWT:Issuer is not configured properly.");
    var audience = builder.Configuration["JWT:Audience"]
        ?? throw new ArgumentException("JWT:Audience is not configured properly.");

    // configure JWT Bearer options
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        // allow some clock drift (+- 30 seconds)
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    // configure events for JWT Bearer (middleware)
    // nếu kco middleware này thì khi 1 AC hoặc RFT đã bị revoke thì vẫn còn dùng đc
    // phải có thì middleware này kiểm tra thấy đã bị revoke thì return về luôn.
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                context.Fail("Missing jti");
                return;
            }

            // lấy dataContext (EF Core) từ DI container
            var db = context.HttpContext.RequestServices.GetService<DataContext>();

            var revoked = await db!.RefreshTokenRecords.AnyAsync(rft =>
                rft.AccessTokenJti == jti &&
                rft.RevokeAtUtc != null
            );
            if (revoked) context.Fail("Token has been revoked");
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// using for API protected routes
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
