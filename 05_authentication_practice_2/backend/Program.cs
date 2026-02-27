using System.IdentityModel.Tokens.Jwt;
using backend.Data;
using backend.Repositories;
using backend.Services;
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

builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddSingleton<TokenService>();

// configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JWT:Key"]
            ?? throw new InvalidOperationException("JWT:Key is not configured");
        var issuer = builder.Configuration["JWT:Issuer"]
            ?? throw new InvalidOperationException("JWT:Issuer is not configured");
        var audience = builder.Configuration["JWT:Audience"]
            ?? throw new InvalidOperationException("JWT:Audience is not configured");

        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)),

            ClockSkew = TimeSpan.Zero
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
                    context.Fail("Missing JTI claim in token");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<DataContext>();

                var refreshToken = await db.RefreshTokenRecords
                    .FirstOrDefaultAsync(rt => rt.AccessTokenJti == jti);

                if (refreshToken != null && !refreshToken.IsActive)
                {
                    context.Fail("Token has been revoked or expired");
                    return;
                }
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
