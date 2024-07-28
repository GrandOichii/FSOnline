using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace FSMatchManager.Matches;

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
    [JsonIgnore] // TODO remove
    public TcpListener TcpListener { get; private set; }
    public int TcpPort { get; private set; }

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
        // config tcp
        TcpListener = new TcpListener(IPAddress.Loopback, 0);
        TcpListener.Start();
        TcpPort = ((IPEndPoint)TcpListener.LocalEndpoint).Port;

        // config bots
        foreach (var bot in Params.Bots) {
            var player = await CreateBotPlayer(bot);
        }
    }

    public async Task<QueuedPlayer> CreateBotPlayer(BotParams botParams) {
        // TODO switch bot type
        // TODO set seed based on config
        // TODO set dealy based on config
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

    public void AddQueuedPlayer(QueuedPlayer player) {
        player.Changed += OnPlayerChanged;
        player.StatusUpdated += OnPlayerStatusUpdated;
        Players.Add(player);
    }

    /// <summary>
    /// Event handler for player changing event
    /// </summary>
    public async Task OnPlayerChanged() {
        if (Changed is not null)
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
        // TODO
    }

    public async Task AddWSPlayer(WebSocket socket) {
        var controller = new IOPlayerController(new WebSocketIOHandler(socket));
        
    }
}