using System.Text;
using backend.Data;
using backend.Service;
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

// register DI container for TokenService
builder.Services.AddSingleton<TokenService>();

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
