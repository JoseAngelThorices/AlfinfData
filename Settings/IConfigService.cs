// 1) Extiende tu interfaz:
public interface IConfigService
{
    string OdooUrl { get; set; }
    string OdooPort { get; set; }

    // Añade estos:
    Task SetCredentialsAsync(string? user, string? pass, string? nameDatabase);
    Task<(string user, string pass, string nameDateBase)> GetCredentialsAsync();

    event EventHandler ConfigChanged;
}

// 2) Implémentalos usando SecureStorage
public class ConfigService : IConfigService
{
    const string UrlKey = "OdooUrl";
    const string PortKey = "OdooPort";
    const string UserKey = "OdooUser";
    const string PassKey = "OdooPass";
    const string NameDataBaseKey = "OdooNameDataBase";
    public event EventHandler? ConfigChanged;

    public string OdooUrl
    {
        get => Preferences.Get(UrlKey, "http://127.0.0.1");
        set
        {
            Preferences.Set(UrlKey, value);
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string OdooPort
    {
        get => Preferences.Get(PortKey, "8069");
        set
        {
            Preferences.Set(PortKey, value);
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task SetCredentialsAsync(string? user, string? pass, string? nameDataBase)
    {
        await SecureStorage.SetAsync(UserKey, user ?? "");
        await SecureStorage.SetAsync(PassKey, pass ?? "");
        await SecureStorage.SetAsync(NameDataBaseKey, nameDataBase ?? "");
    }

    public async Task<(string user, string pass, string nameDateBase)> GetCredentialsAsync()
    {
        var user = await SecureStorage.GetAsync(UserKey) ?? "";
        var pass = await SecureStorage.GetAsync(PassKey) ?? "";
        var nameDateBase = await SecureStorage.GetAsync(NameDataBaseKey) ?? "";

        return (user, pass, nameDateBase);
    }
}
