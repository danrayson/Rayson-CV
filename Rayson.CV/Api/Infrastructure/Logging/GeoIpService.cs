using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public interface IGeoIpService
{
    string? GetCountryCode(string ipAddress);
    Task EnsureDatabaseAsync(CancellationToken cancellationToken = default);
}

public class GeoIpService : IGeoIpService
{
    private readonly ILogger<GeoIpService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _databasePath;
    private readonly string _cdnUrl;
    private DatabaseReader? _reader;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    private static readonly Dictionary<string, string> CountryNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["US"] = "United States", ["GB"] = "United Kingdom", ["DE"] = "Germany",
        ["FR"] = "France", ["AU"] = "Australia", ["CA"] = "Canada",
        ["JP"] = "Japan", ["CN"] = "China", ["IN"] = "India",
        ["BR"] = "Brazil", ["MX"] = "Mexico", ["ES"] = "Spain",
        ["IT"] = "Italy", ["NL"] = "Netherlands", ["SE"] = "Sweden",
        ["NO"] = "Norway", ["DK"] = "Denmark", ["FI"] = "Finland",
        ["PL"] = "Poland", ["RU"] = "Russia", ["KR"] = "South Korea",
        ["SG"] = "Singapore", ["HK"] = "Hong Kong", ["NZ"] = "New Zealand",
        ["IE"] = "Ireland", ["CH"] = "Switzerland", ["AT"] = "Austria",
        ["BE"] = "Belgium", ["PT"] = "Portugal", ["CZ"] = "Czech Republic",
        ["HU"] = "Hungary", ["RO"] = "Romania", ["UA"] = "Ukraine",
        ["TR"] = "Turkey", ["IL"] = "Israel", ["AE"] = "United Arab Emirates",
        ["ZA"] = "South Africa", ["EG"] = "Egypt", ["NG"] = "Nigeria",
        ["KE"] = "Kenya", ["AR"] = "Argentina", ["CL"] = "Chile",
        ["CO"] = "Colombia", ["PE"] = "Peru", ["VE"] = "Venezuela",
        ["TW"] = "Taiwan", ["TH"] = "Thailand", ["MY"] = "Malaysia",
        ["ID"] = "Indonesia", ["PH"] = "Philippines", ["VN"] = "Vietnam"
    };

    public GeoIpService(
        IConfiguration config,
        ILogger<GeoIpService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _databasePath = config["GeoIP:DatabasePath"] ?? "App_Data/GeoLite2-Country.mmdb";
        _cdnUrl = "https://cdn.jsdelivr.net/npm/geolite2-country/GeoLite2-Country.mmdb.gz";
    }

    public async Task EnsureDatabaseAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized) return;

        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized) return;

            var directory = Path.GetDirectoryName(_databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_databasePath))
            {
                _logger.LogInformation("Downloading GeoLite2 database from {Url} to {Path}", _cdnUrl, _databasePath);
                await DownloadDatabaseAsync(cancellationToken);
            }

            InitializeReader();
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task DownloadDatabaseAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var gzPath = _databasePath + ".gz";
        using (var response = await httpClient.GetAsync(_cdnUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            await using var fs = new FileStream(gzPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            await response.Content.CopyToAsync(fs, cancellationToken);
        }

        await using var gzStream = File.OpenRead(gzPath);
        using var decompressor = new System.IO.Compression.GZipStream(gzStream, System.IO.Compression.CompressionMode.Decompress);
        await using var outStream = File.Create(_databasePath);
        await decompressor.CopyToAsync(outStream, cancellationToken);

        File.Delete(gzPath);
        _logger.LogInformation("GeoLite2 database downloaded and extracted successfully");
    }

    private void InitializeReader()
    {
        if (!File.Exists(_databasePath))
        {
            _logger.LogWarning("GeoIP database not found at {Path}. Country lookup disabled.", _databasePath);
            return;
        }

        try
        {
            _reader = new DatabaseReader(_databasePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to initialize GeoIP database at {Path}", _databasePath);
        }
    }

    public string? GetCountryCode(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress)) return null;

        var cfCountry = _httpContextAccessor.HttpContext?
            .Request.Headers["CF-IPCountry"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfCountry))
        {
            return cfCountry;
        }

        if (IsPrivateIp(ipAddress))
            return null;

        EnsureInitialized();

        if (_reader == null) return null;

        try
        {
            var response = _reader.Country(ipAddress);
            return response.Country.IsoCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "GeoIP lookup failed for {Ip}", ipAddress);
            return null;
        }
    }

    public string? GetCountryName(string ipAddress)
    {
        var code = GetCountryCode(ipAddress);
        return code != null && CountryNames.TryGetValue(code, out var name) ? name : code;
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            InitializeReader();
            _initialized = true;
        }
    }

    private static bool IsPrivateIp(string ip)
    {
        return ip == "127.0.0.1" || ip == "::1" ||
               ip.StartsWith("192.168.", StringComparison.OrdinalIgnoreCase) ||
               ip.StartsWith("10.", StringComparison.OrdinalIgnoreCase) ||
               ip.StartsWith("172.16.", StringComparison.OrdinalIgnoreCase) ||
               ip.StartsWith("172.31.", StringComparison.OrdinalIgnoreCase);
    }
}
