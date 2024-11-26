using Microsoft.EntityFrameworkCore;
using TCGGAPI;
using TCGGAPI.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<TCGGDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/game", (GameManager gm, int coinToss) =>
{
    return gm.StartMatch(coinToss);
});

app.MapGet("/reset", (GameManager gm, int coinToss) =>
{
    gm.RestartMatch(coinToss);
});

app.MapGet("/getMatch", (GameManager gm) => {
    return gm.Match;
});

app.MapGet("/draw", (GameManager gm, int PlayerId) =>
{
    return gm.DrawCard(PlayerId);
});

app.MapPost("/playCard", (GameManager gm, int playerId, int cardId) =>
{
    return gm.PlayCardToBoard(playerId, cardId);
});

app.MapPost("/attackCard", (GameManager gm, int attackCardId, int defenseCardId, int playerId) =>
{
    gm.AttackCard(attackCardId, defenseCardId, playerId);
});

// TODO: WIN OR LOSE condition
app.MapPost("/attackPlayer", (GameManager gm, int playerId, int cardId) =>
{
    return gm.AttackPlayer(playerId, cardId);
});

app.MapPost("/endTurn", (GameManager gm, int playerId) =>
{
   gm.EndTurn(playerId); 
});

app.Run();

