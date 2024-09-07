using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using FSCore.Cards.CardMasters;

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

public class MatchProcess(CreateMatchParams creationParams, ICardService cardService)
{
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

    public readonly ICardService _cardService = cardService;

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
        System.Console.WriteLine("Configuring match..");
        System.Console.WriteLine("Creating bots");
        // config bots
        foreach (var bot in Params.Bots) {
            System.Console.WriteLine("Creating bot " + bot.Name);
            await CreateBotPlayer(bot);
            System.Console.WriteLine("Bot " + bot.Name + " created!");
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
        System.Console.WriteLine("Adding bot to the queue");

        AddQueuedPlayer(result);

        return result;
    } 

    private readonly object _addPlayerLock = new();

    public void AddQueuedPlayer(QueuedPlayer player) {
        player.Changed += OnPlayerChanged;
        player.StatusUpdated += OnPlayerStatusUpdated;

        System.Console.WriteLine("Lock");
        lock (_addPlayerLock) {
            System.Console.WriteLine("Before add");
            Players.Add(player);
            System.Console.WriteLine("After add");
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
        System.Console.WriteLine("Refreshing connections");
        var newPlayers = new List<QueuedPlayer>();
        foreach (var player in Players) {
            var valid = await player.Checker.Check();
            System.Console.WriteLine(player.Checker + " " + valid);
            if (!valid) continue;
            newPlayers.Add(player);
        }
        System.Console.WriteLine($"Before: {Players.Count}\tAfter {newPlayers.Count}");
        var prevCount = Players.Count;
        Players = newPlayers;
        // TODO update view
        return Players.Count == prevCount;
    }

    public async Task Run() {
        // TODO seed
        System.Console.WriteLine("Configuring match");
        
        // var cm = new DBCardMaster(_cardService);

        var cm = new FileCardMaster();
        cm.Load("../cards/b");
        // cm.Load("../cards/b2");

        Match = new(Params.Config, _rng.Next(), cm, File.ReadAllText("../core.lua"));
        Match.Logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger("Match");

        // TODO match view

        await CreatePlayerControllers(Match);
        await SetStatus(MatchStatus.IN_PROGRESS);

        try {
            System.Console.WriteLine("Match started");
            await Match.Run();
            await SetStatus(MatchStatus.FINISHED);
            System.Console.WriteLine("Match finished!");
            // TODO add back
            // Record.WinnerName = Match.Winner!.Name;
            // await _matchService.ServiceStatusUpdated(this);
        } catch (Exception e) {
            System.Console.WriteLine("Match crashed");
            await SetStatus(MatchStatus.CRASHED);
            // TODO add back
            // Record.ExceptionMessage = e.Message;
            // if (e.InnerException is not null)
            //     Record.InnerExceptionMessage = e.InnerException.Message;      
            System.Console.WriteLine("Match crashed");
            PrintException(e);
            // TODO add back
            // await Match.View.End();
        }
    }

    public async Task SetStatus(MatchStatus status) {
        Status = status;
        System.Console.WriteLine("Setting match status to " + status);
        // TODO add back
        // await _matchService.ServiceStatusUpdated(this);
        
        if (Changed is not null)
            await Changed.Invoke(ID.ToString());

    }

    public async Task CreatePlayerControllers(Match match) {
        System.Console.WriteLine("Creating player controllers");
        foreach (var player in Players) {
            var baseController = player!.Controller;
            // var record = new PlayerRecord() {
            //     Name = player.Name!,
            //     Deck = player.Deck!,
            // };
            // Record.Players.Add(record);

            // var controller = new RecordingPlayerController(baseController, record);
            var controller = baseController;

            System.Console.WriteLine($"Adding controller for {player.GetName()}");
            await match.AddPlayer(player.GetName(), controller!, player.GetCharacterKey());
            System.Console.WriteLine("Added controller for " + player.GetName());
        }
        System.Console.WriteLine("Player controller created");
    }

    public async Task AddWSPlayer(WebSocket socket) {
        System.Console.WriteLine("Adding WebSocket player");
        var controller = new IOPlayerController(new WebSocketIOHandler(socket));
        var added = await AddPlayer(controller, new WebSocketConnectionChecker(socket));
        if (!added) {
            System.Console.WriteLine("Failed to read player info, not adding WebSocket player to the match");
            return;
        }

        System.Console.WriteLine("WebSocket player added, adding the socket to the wait loop");
        await Finish(socket);
    }

    public async Task<bool> AddPlayer(IOPlayerController controller, IConnectionChecker checker) {
        System.Console.WriteLine("Request to add a real player");
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
        System.Console.WriteLine($"Read info of player " + player.GetName());
        // TODO? change
        player.Status = QueuedPlayerStatus.READY;
        
        AddQueuedPlayer(player);

        if (Players.Count(p => !p.IsBot) > 1) return true;

        // TODO assign as owner player
        System.Console.WriteLine("Player " + player.GetName() + " assigned as the main player");
        _ = WaitForStart(checker);

        return true;
    }

    public async Task WaitForStart(IConnectionChecker checker) {
        while (true) {
            await checker.Write("mpa");
            System.Console.WriteLine("Reading from main player");
            var msg = await checker.Read();
            System.Console.WriteLine("Read: " + msg);
            if (msg != "start") {
                // TODO better exception
                throw new Exception("Expected to read \"start\" from match owner, but got " + msg);
            }

            if (!await CanRun()) continue;
            break;
        }
        System.Console.WriteLine("Match can start");
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