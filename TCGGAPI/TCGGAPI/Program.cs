using Microsoft.EntityFrameworkCore;
using TCGGAPI;
using TCGGAPI.Data;
using TCGGAPI.DTO;
using TCGGAPI.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<TCGGDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<IMatchService, MatchService>();
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
   gm.StartMatch(coinToss);
});

app.MapGet("/reset", (GameManager gm, int coinToss) =>
{
    gm.RestartMatch(coinToss);
});

app.MapGet("/getMatch", (GameManager gm) => {
    return gm.GetMatch();
});


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

app.MapGet("/draw", (GameManager gm, int PlayerId) =>
{
    return gm.DrawCard(PlayerId);
});

app.MapGet("/getHand", (GameManager gm, int PlayerId) =>
{
    return gm.GetHand(PlayerId);
});

app.MapPost("/playCard", (GameManager gm, int playerId, int cardId) =>
{
    gm.PlayCardToBoard(playerId, cardId);
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

