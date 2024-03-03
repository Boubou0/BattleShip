using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Diagnostics;
using BattleShip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCors()
    ;
builder.Services
    .AddSingleton<BoardService>()
    .AddSingleton<ActionService>()
    .AddSingleton<GameService>()
    .AddScoped<LeaderboardService>()
    ;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=leaderboard.db"));

builder.Services.AddAuthentication((options) => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c => {
        c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Auth0:Audience"],
            ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}"
        };
        c.TokenValidationParameters.NameClaimType = "https://dev-6bxro7e01zwus67q.eu.auth0.comname";
    });

builder.Services.AddAuthorization(options =>{});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Utilise le middleware Auth0

app.UseCors(c => {
    c.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

BattleShip.API.Route.RegisterRoutes(app);

app.Run();