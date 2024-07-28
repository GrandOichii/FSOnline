using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace FSMatchManager.Matches;


/// <summary>
/// Match process status
/// </summary>
public enum MatchStatus {
    /// <summary>
    /// Waiting for players to connect
    /// </summary>
    WAITING_FOR_PLAYERS,

    /// <summary>
    /// Ready to start
    /// </summary>
    READY_TO_START,

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

public class MatchProcess(CreateMatchParams creationParams)
{
    private readonly Random _rng = new();

    /// <summary>
    /// Match process ID
    /// </summary>
    public Guid ID { get; } = Guid.NewGuid();
    /// <summary>
    /// Match creation parameters
    /// </summary>
    public CreateMatchParams Params { get; } = creationParams;
    /// <summary>
    /// Match status
    /// </summary>
    public MatchStatus Status { get; private set; }
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

        // config bots
        foreach (var bot in Params.Bots) {
            await CreateBotPlayer(bot);
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

    private readonly object _addPlayerLock;

    public void AddQueuedPlayer(QueuedPlayer player) {
        player.Changed += OnPlayerChanged;
        player.StatusUpdated += OnPlayerStatusUpdated;

        // lock (_addPlayerLock) {
            System.Console.WriteLine("ADD PLAYER");
            Players.Add(player);
            System.Console.WriteLine("PLAYER ADDED");
        // }
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

    public async Task RefreshConnections() {
        var newPlayers = new List<QueuedPlayer>();
        foreach (var player in Players) {
            var valid = await player.Checker.Check();
            if (!valid) continue;
            newPlayers.Add(player);
        }
        Players = newPlayers;
    }

    public async Task Run() {
        // TODO seed

        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        Match = new(Params.Config, _rng.Next(), cm, File.ReadAllText("../core.lua"));
        // TODO match view

        await CreatePlayerControllers(Match);
        await SetStatus(MatchStatus.IN_PROGRESS);

        try {
            await Match.Run();
            await SetStatus(MatchStatus.FINISHED);
            // TODO add back
            // Record.WinnerName = Match.Winner!.Name;
            // await _matchService.ServiceStatusUpdated(this);
        } catch (Exception e) {
            await SetStatus(MatchStatus.CRASHED);
            // TODO add back
            // Record.ExceptionMessage = e.Message;
            // if (e.InnerException is not null)
            //     Record.InnerExceptionMessage = e.InnerException.Message;      
            System.Console.WriteLine(e.Message);      
            System.Console.WriteLine(e.StackTrace);
            System.Console.WriteLine("Match crashed");
            // TODO add back
            // await Match.View.End();
        }


    }

    public async Task SetStatus(MatchStatus status) {
        Status = status;
        // TODO add back
        // await _matchService.ServiceStatusUpdated(this);
        
        if (Changed is not null)
            await Changed.Invoke(ID.ToString());

    }

    public async Task CreatePlayerControllers(Match match) {
        foreach (var player in Players) {
            var baseController = player!.Controller;
            // var record = new PlayerRecord() {
            //     Name = player.Name!,
            //     Deck = player.Deck!,
            // };
            // Record.Players.Add(record);

            // var controller = new RecordingPlayerController(baseController, record);
            var controller = baseController;

            await match.AddPlayer(player.GetName(), controller!);
        }
    }

    public async Task AddWSPlayer(WebSocket socket) {
        var controller = new IOPlayerController(new WebSocketIOHandler(socket));
        await AddPlayer(controller, new WebSocketConnectionChecker(socket));
        await Finish(socket);
    }

    public async Task AddPlayer(IOPlayerController controller, IConnectionChecker checker) {
        var player = new QueuedPlayer(
            controller,
            checker,
            false
        );

        var errMsg = await player.ReadPlayerInfo(this, checker);
        if (!string.IsNullOrEmpty(errMsg)) {
            await checker.Write(errMsg);
            return;
        }

        AddQueuedPlayer(player);

        if (Players.Count > 1) return;

        // TODO assign as owner player
        _ = WaitForStart(checker);
    }

    public async Task WaitForStart(IConnectionChecker checker) {
        while (true) {
            var msg = await checker.Read();
            if (msg != "start") {
                // TODO better exception
                throw new Exception("Expected to read \"start\" from match owner, but got " + msg);
            }

            if (!await CanRun()) continue;
            break;
        }
        await Run();
    }

    public async Task<bool> CanRun() {
        await RefreshConnections();
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
}