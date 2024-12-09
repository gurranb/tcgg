using Microsoft.EntityFrameworkCore;
using TCGGAPI;
using TCGGAPI.Data;
using TCGGAPI.DTO;
using TCGGAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services
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

// Register application services
builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

// API Endpoints

// Game Management
app.MapGet("/game", (GameManager gm, int coinToss) => gm.StartMatch(coinToss));
app.MapGet("/reset", (GameManager gm, int coinToss) => gm.RestartMatch(coinToss));
app.MapGet("/getMatch", (GameManager gm) => gm.GetMatch());
app.MapGet("/getBoard", (GameManager gm) =>
{
    var board = gm.GetBoard();
    var player1Dto = new PlayerDto
    {
        Name = board.Player1.Name,
        MatchDeckAmount = board.Player1.MatchDeck.Cards.Count
    };
    var player2Dto = new PlayerDto
    {
        Name = board.Player2.Name,
        MatchDeckAmount = board.Player2.MatchDeck.Cards.Count
    };
    var boardDto = new BoardDto
    {
        Player1 = player1Dto,
        Player2 = player2Dto,
        Player1Field = board.Player1Field,
        Player2Field = board.Player2Field,
        Turns = board.Turns
    };
    return boardDto;
});

// Player Actions
app.MapGet("/draw", (GameManager gm, int playerId) => gm.DrawCard(playerId));
app.MapGet("/startTurn", (GameManager gm, int playerId) => gm.StartTurn(playerId));
app.MapGet("/drawRndCard", (GameManager gm, int playerId) => gm.DrawRandomCard(playerId));
app.MapGet("/drawMultipleCards", (GameManager gm, int playerId, int amount) => gm.DrawMultipleCards(playerId, amount));
app.MapGet("/getHand", (GameManager gm, int playerId) => gm.GetHand(playerId));
app.MapPost("/endTurn", (GameManager gm, int playerId) => gm.EndTurn(playerId));

// Card Actions
app.MapPost("/playCard", (GameManager gm, int playerId, int cardId) => gm.PlayCardToBoard(playerId, cardId));
app.MapPost("/attackCard", (GameManager gm, int attackCardId, int defenseCardId, int playerId) => gm.AttackCard(attackCardId, defenseCardId, playerId));
app.MapPost("/attackPlayer", (GameManager gm, int playerId, int cardId) => gm.AttackPlayer(playerId, cardId));

// Run the application
app.Run();
