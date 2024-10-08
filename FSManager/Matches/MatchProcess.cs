using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using FSCore.Cards.CardMasters;
using Microsoft.OpenApi.Writers;

namespace FSManager.Matches;


/// <summary>
/// Match process status
/// </summary>
public enum MatchStatus {
    /// <summary>
    /// Waiting for players to connect
    /// </summary>
    WAITING_FOR_PLAYERS,

    /// <summary>
    /// Match process is in progress
    /// </summary>
    IN_PROGRESS,

    /// <summary>
    /// Match process is finished without error
    /// </summary>
    FINISHED,

    /// <summary>
    /// Match process crashed during running
    /// </summary>
    CRASHED
}

public class MatchProcess(
    CreateMatchParams creationParams, 
    ICardService cardService, 
    ILogger<MatchService> _serviceLogger,
    ILogger<MatchProcess> _matchLogger
){
    private readonly Random _rng = new();

    /// <summary>
    /// Match process ID
    /// </summary>
    public Guid ID { get; } = Guid.NewGuid();
    /// <summary>
    /// Match creation parameters
    /// </summary>
    [JsonIgnore] // TODO remove
    public CreateMatchParams Params { get; } = creationParams;
    /// <summary>
    /// Match status
    /// </summary>
    public MatchStatus Status { get; private set; } = MatchStatus.WAITING_FOR_PLAYERS;
    /// <summary>
    /// Match
    /// </summary>
    [JsonIgnore] // TODO remove
    public Match? Match { get; private set; }

    /// <summary>
    /// Password hash
    /// </summary>
    private readonly string _passwordHash = string.IsNullOrEmpty(creationParams.Password)
        ? ""
        : BCrypt.Net.BCrypt.HashPassword(creationParams.Password);

    /// <summary>
    /// Match process changed delegate
    /// </summary>
    /// <param name="matchId">Match ID</param>
    public delegate Task MatchProcessChanged(string matchId);

    /// <summary>
    /// Match process changed event
    /// </summary>
    public event MatchProcessChanged? Changed;

    public List<QueuedPlayer> Players { get; private set; } = [];

    private readonly ICardService _cardService = cardService;
    private readonly ILogger<MatchService> _logger = _serviceLogger;
    private IDisposable? _logScope = null;

    // [JsonIgnore] // TODO remove
    // public TcpListener TcpListener { get; private set; }
    // public int TcpPort { get; private set; }

    /// <summary>
    /// Checks the match password
    /// </summary>
    /// <param name="password">Potential password</param>
    /// <returns>True if the potential password matches the match password, else false</returns>
    public bool CheckPassword(string password) {
        if (!RequiresPassword()) return true;

        return BCrypt.Net.BCrypt.Verify(password, _passwordHash);
    }

    /// <summary>
    /// Check whether the match requires a password
    /// </summary>
    /// <returns>True is password is required, else false</returns>
    public bool RequiresPassword() => !string.IsNullOrEmpty(_passwordHash);

    public async Task Configure() {
        // configure tcp
        // TcpListener = new TcpListener(IPAddress.Loopback, 0);
        // TcpListener.Start();
        // TcpPort = ((IPEndPoint)TcpListener.LocalEndpoint).Port;
        _logScope = _logger.BeginScope("{Name}:{Id} configuration", nameof(MatchProcess), ID.ToString());

        _logger.LogInformation("Configuring match");
        _logger.LogDebug("Creating bots");

        foreach (var bot in Params.Bots) {
            _logger.LogDebug("Creating bot {BotName}", bot.Name);

            await CreateBotPlayer(bot);

            _logger.LogDebug("Bot {BotName} created!", bot.Name);
        }
    }

    public async Task<QueuedPlayer> CreateBotPlayer(BotParams botParams) {
        // TODO switch bot type
        // TODO set seed based on config
        // TODO set delay based on config
        var controller = new RandomPlayerController(_rng.Next(), 100);

        var name = botParams.Name;
        var result = new QueuedPlayer(
            controller,
            new BotConnectionChecker(),
            true
        ) {
            Name = name,
            Status = QueuedPlayerStatus.READY
        };

        AddQueuedPlayer(result);

        return result;
    } 

    private readonly object _addPlayerLock = new();

    public void AddQueuedPlayer(QueuedPlayer player) {
        player.Changed += OnPlayerChanged;
        player.StatusUpdated += OnPlayerStatusUpdated;

        lock (_addPlayerLock) {
            Players.Add(player);
        }
    }

    /// <summary>
    /// Event handler for player changing event
    /// </summary>
    public async Task OnPlayerChanged() {
        if (Changed is null) return;

        await Changed.Invoke(ID.ToString());
    }

    /// <summary>
    /// Event handler for player status changing
    /// </summary>
    public async Task OnPlayerStatusUpdated() {
        // TODO
    }

    public async Task<bool> RefreshConnections() {
        _logger.LogDebug("Refreshing connections");
        var newPlayers = new List<QueuedPlayer>();
        foreach (var player in Players) {
            var valid = await player.Checker.Check();
            _logger.LogDebug("Player {PlayerName} connection valid: {Valid}", player.GetName(), valid);
            if (!valid) continue;
            newPlayers.Add(player);
        }
        _logger.LogDebug("Before check: {OldPlayerCount}, after check: {NewPlayerCount}", Players.Count, newPlayers.Count);

        var prevCount = Players.Count;
        Players = newPlayers;
        // TODO update view
        return Players.Count == prevCount;
    }

    public async Task Run() {
        _logScope?.Dispose();
        _logScope = _logger.BeginScope("{Name}:{Id}", nameof(MatchProcess), ID.ToString());
        
        // TODO seed
        _logger.LogDebug("Initial match config");
        
        // var cm = new DBCardMaster(_cardService);

        var cm = new FileCardMaster();
        cm.Load("../cards/b");
        // cm.Load("../cards/b2");

        Match = new(Params.Config, _rng.Next(), cm, File.ReadAllText("../core.lua"));
        Match.Logger = _matchLogger;

        // TODO match view

        await CreatePlayerControllers(Match);
        await SetStatus(MatchStatus.IN_PROGRESS);

        try {
            _logger.LogInformation("Match started");

            await Match.Run();
            await SetStatus(MatchStatus.FINISHED);

            _logger.LogInformation("Match finished");

            // TODO add back
            // Record.WinnerName = Match.Winner!.Name;
            // await _matchService.ServiceStatusUpdated(this);
        } catch (Exception e) {
            using var scope = _logger.BeginScope(new Dictionary<string, object> {
                { "Exceptions", ToExceptionList(e)}
            });
            _logger.LogError("Match crashed");

            await SetStatus(MatchStatus.CRASHED);
            // TODO add back
            // Record.ExceptionMessage = e.Message;
            // if (e.InnerException is not null)
            //     Record.InnerExceptionMessage = e.InnerException.Message;      

            // TODO add back
            // await Match.View.End();
        }
    }

    private static List<Exception> ToExceptionList(Exception e) {
        var result = new List<Exception>();
        var ex = e;
        while (ex is not null) {
            result.Add(ex);
            ex = ex.InnerException;
        }
        return result;
    }

    public async Task SetStatus(MatchStatus status) {
        Status = status;
        _logger.LogDebug("Setting match status to {MatchStatus}", status);
        // TODO add back
        // await _matchService.ServiceStatusUpdated(this);
        
        if (Changed is not null)
            await Changed.Invoke(ID.ToString());

    }

    public async Task CreatePlayerControllers(Match match) {
        _logger.LogDebug("Creating player controllers");
        foreach (var player in Players) {
            var baseController = player!.Controller;
            // var record = new PlayerRecord() {
            //     Name = player.Name!,
            //     Deck = player.Deck!,
            // };
            // Record.Players.Add(record);

            // var controller = new RecordingPlayerController(baseController, record);
            var controller = baseController;

            _logger.LogDebug("Adding controller for {PlayerName}", player.GetName());

            await match.AddPlayer(player.GetName(), controller!, player.GetCharacterKey());

            _logger.LogInformation("Added controller for {PlayerName}", player.GetName());
        }
        _logger.LogDebug("Player controller created");
    }

    public async Task AddWSPlayer(WebSocket socket) {
        _logger.LogDebug("Adding WebSocket player");
        var controller = new IOPlayerController(new WebSocketIOHandler(socket));
        var added = await AddPlayer(controller, new WebSocketConnectionChecker(socket));
        if (!added) {
            _logger.LogWarning("Failed to read player info, not adding WebSocket player to the match");
            return;
        }

        _logger.LogDebug("WebSocket player added, adding the socket to the wait loop");
        await Finish(socket);
    }

    public async Task<bool> AddPlayer(IOPlayerController controller, IConnectionChecker checker) {
        _logger.LogDebug("Request to add a real player");
        var player = new QueuedPlayer(
            controller,
            checker,
            false
        );

        var errMsg = await player.ReadPlayerInfo(this, checker);
        if (!string.IsNullOrEmpty(errMsg)) {
            await checker.Write(errMsg);
            return false;
        }
        _logger.LogDebug("Received info of player {PlayerName}", player.GetName());
        
        // TODO? change
        player.Status = QueuedPlayerStatus.READY;
        
        AddQueuedPlayer(player);

        if (Players.Count(p => !p.IsBot) > 1) return true;

        // TODO assign as owner player
        _logger.LogInformation("Player {PlayerName} assigned as the main player", player.GetName());
        _ = WaitForStart(checker);

        return true;
    }

    public async Task WaitForStart(IConnectionChecker checker) {
        while (true) {
            await checker.Write("mpa");
            _logger.LogDebug("Requested Match Process Arguments from main player");
            var msg = await checker.Read();
            if (msg != "start") {
                // TODO better exception
                throw new Exception("Expected to read \"start\" from match owner, but got " + msg);
            }

            if (!await CanRun()) continue;
            break;
        }
        _logger.LogDebug("Match can start");
        await Run();
    }

    public async Task<bool> CanRun() {
        var nobodyDisconnected = await RefreshConnections();
        if (!nobodyDisconnected) return false;
        if (Players.Count < 2) return false;

        return Players.All(p => p.Status == QueuedPlayerStatus.READY);
    }

    /// <summary>
    /// Runs a loop for a WebSocket connection to keep it from disconnecting until the match is finished
    /// </summary>
    /// <param name="socket">WebSocket connection</param>
    public async Task Finish(WebSocket socket) {
        while (Status <= MatchStatus.IN_PROGRESS && socket.State == WebSocketState.Open) {
            await Task.Delay(200);
        }
    }

    /// <summary>
    /// Checks whether a new player can be added to the match
    /// </summary>
    /// <returns>True if a new player can be added, otherwise returns false</returns>
    public bool CanAddPlayer() {
        return Status == MatchStatus.WAITING_FOR_PLAYERS && Players.Count < Params.Config.MaxPlayerCount;
    }


    static void PrintException(Exception e) {
        var ex = e;
        while (ex is not null) {
            System.Console.WriteLine(ex.ToString());
            ex = ex.InnerException;
        }
    }

}