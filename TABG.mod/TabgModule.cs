using System.Text;
using WindowsGSH.Core.Modules;

namespace WindowsGSH.Modules.TABG;

public sealed class TabgModule : ManifestBackedGameServerModule, IModuleExistingServerImportCapability
{
    private const string ConfigFileName = "game_settings.txt";
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    private static readonly SettingMapping[] Mappings =
    [
        new("ServerName", "server.name", "TABG Server"),
        new("Description", "server.description", "Community Server"),
        new("Password", "server.password", ""),
        new("MaxPlayers", "server.maxPlayers", "16"),
        new("Relay", "network.relay", "true"),
        new("Port", "network.port", "7777"),
        new("PlayersToStart", "gameplay.playersToStart", "2"),
        new("MinPlayersToForceStart", "gameplay.minimumPlayersToForceStart", "1"),
        new("TeamMode", "gameplay.teamMode", "solo"),
        new("AllowSpectating", "gameplay.allowSpectating", "true")
    ];

    public bool CanImport(string path) => ExistingInstallImport.CanImport(this, path);

    public Task<ModuleExistingServerImportProbe> PreviewImportAsync(string path, CancellationToken cancellationToken) =>
        ExistingInstallImport.PreviewAsync(this, path, cancellationToken);

    public override Task<IReadOnlyDictionary<string, object?>> ReadConfigFileSettingsAsync(
        ServerInstance instance,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = Path.Combine(instance.InstallPath, ConfigFileName);
        if (!File.Exists(path))
        {
            return Task.FromResult<IReadOnlyDictionary<string, object?>>(new Dictionary<string, object?>());
        }

        var settings = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadLines(path))
        {
            if (!TryParseSetting(line, out var key, out var value))
            {
                continue;
            }

            var mapping = Mappings.FirstOrDefault(candidate => candidate.ConfigKey.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                settings[mapping.SettingKey] = value;
            }
        }

        return Task.FromResult<IReadOnlyDictionary<string, object?>>(settings);
    }

    public override Task WriteConfigFileSettingsAsync(ServerInstance instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = Path.Combine(instance.InstallPath, ConfigFileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "TABG did not install game_settings.txt. Run Verify Files, then try again.",
                path);
        }

        var lines = File.ReadAllLines(path).ToList();
        foreach (var mapping in Mappings)
        {
            SetValue(lines, mapping.ConfigKey, GetSetting(instance, mapping.SettingKey, mapping.DefaultValue));
        }

        var temporaryPath = path + ".windowsgsh.tmp";
        try
        {
            File.WriteAllLines(temporaryPath, lines, Utf8NoBom);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }

        return Task.CompletedTask;
    }

    private static void SetValue(List<string> lines, string key, string value)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            if (TryParseSetting(lines[index], out var existingKey, out _) && existingKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                lines[index] = $"{key}={value}";
                return;
            }
        }

        lines.Add($"{key}={value}");
    }

    private static bool TryParseSetting(string line, out string key, out string value)
    {
        key = string.Empty;
        value = string.Empty;
        var trimmed = line.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith("//", StringComparison.Ordinal) || trimmed.StartsWith('#'))
        {
            return false;
        }

        var separator = trimmed.IndexOf('=');
        if (separator <= 0)
        {
            return false;
        }

        key = trimmed[..separator].Trim();
        value = trimmed[(separator + 1)..].Trim();
        return true;
    }

    private sealed record SettingMapping(string ConfigKey, string SettingKey, string DefaultValue);
}
