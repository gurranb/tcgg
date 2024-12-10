using Microsoft.EntityFrameworkCore;
using TCGGAPI;
using TCGGAPI.Data;
using TCGGAPI.DTO;
using TCGGAPI.Models;
using TCGGAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
        policy.WithOrigins("http://localhost:5173") // Frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddDbContext<TCGGDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application services
builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

// API Endpoints

// Game Management Endpoints
app.MapGet("/game", (GameManager gm, int coinToss) => gm.StartMatch(coinToss))
   .WithName("StartMatch")
   .Produces<Match>();

app.MapGet("/reset", (GameManager gm, int coinToss) => gm.RestartMatch(coinToss))
   .WithName("RestartMatch");

app.MapGet("/getMatch", (GameManager gm) => gm.GetMatch())
   .WithName("GetMatch")
   .Produces<Match>();

app.MapGet("/getBoard", (GameManager gm) =>
{
    var board = gm.GetBoard();
    var boardDto = new BoardDto
    {
        Player1 = new PlayerDto
        {
            Name = board.Player1.Name,
            MatchDeckAmount = board.Player1.MatchDeck.Cards.Count
        },
        Player2 = new PlayerDto
        {
            Name = board.Player2.Name,
            MatchDeckAmount = board.Player2.MatchDeck.Cards.Count
        },
        Player1Field = board.Player1Field,
        Player2Field = board.Player2Field,
        Turns = board.Turns
    };
    return boardDto;
}).WithName("GetBoard")
  .Produces<BoardDto>();

// Player Action Endpoints
app.MapGet("/draw", (GameManager gm, int playerId) => gm.DrawCard(playerId))
   .WithName("DrawCard");

app.MapGet("/startTurn", (GameManager gm, int playerId) => gm.StartTurn(playerId))
   .WithName("StartTurn");

app.MapGet("/drawRndCard", (GameManager gm, int playerId) => gm.DrawRandomCard(playerId))
   .WithName("DrawRandomCard");

app.MapGet("/drawMultipleCards", (GameManager gm, int playerId, int amount) => gm.DrawMultipleCards(playerId, amount))
   .WithName("DrawMultipleCards");

app.MapGet("/getHand", (GameManager gm, int playerId) => gm.GetHand(playerId))
   .WithName("GetHand");

// End turn action
app.MapPost("/endTurn", (GameManager gm, int playerId) => gm.EndTurn(playerId))
   .WithName("EndTurn");

// Card Action Endpoints
app.MapPost("/playCard", (GameManager gm, int playerId, int cardId) => gm.PlayCardToBoard(playerId, cardId))
   .WithName("PlayCard");

app.MapPost("/attackCard", (GameManager gm, int attackCardId, int defenseCardId, int playerId) => gm.AttackCard(attackCardId, defenseCardId, playerId))
   .WithName("AttackCard");

app.MapPost("/attackPlayer", (GameManager gm, int playerId, int cardId) => gm.AttackPlayer(playerId, cardId))
   .WithName("AttackPlayer");

app.Run();