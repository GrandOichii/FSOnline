namespace FSMatchManager.Matches;

public class MatchProcess(CreateMatch config) {
    /// <summary>
    /// Match process ID
    /// </summary>
    public Guid ID { get; } = Guid.NewGuid();
    public CreateMatch Config { get; } = config;

    /// <summary>
    /// Password hash
    /// </summary>
    private readonly string _passHash = string.IsNullOrEmpty(config.Password) ? "" : BCrypt.Net.BCrypt.HashPassword(config.Password);


    /// <summary>
    /// Checks the match password
    /// </summary>
    /// <param name="password">Potential password</param>
    /// <returns>True if the potential password matches the match password, else false</returns>
    public bool CheckPassword(string password) {
        if (!RequiresPassword()) return true;

        return BCrypt.Net.BCrypt.Verify(password, _passHash);
    }

    /// <summary>
    /// Check whether the match requires a password
    /// </summary>
    /// <returns>True is password is required, else false</returns>
    public bool RequiresPassword() => !string.IsNullOrEmpty(_passHash);

}