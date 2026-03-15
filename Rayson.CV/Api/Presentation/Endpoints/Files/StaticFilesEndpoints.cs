namespace Presentation.Endpoints.Files;

public static class StaticFilesEndpoints
{
    private static readonly Dictionary<string, (string Filename, string ContentType)> AppFiles = new()
    {
        ["win"] = ("RaysonCV-Setup.exe", "application/x-msdownload"),
        ["linux"] = ("RaysonCV.AppImage", "application/x-iso9660-image"),
        ["mac"] = ("RaysonCV.dmg", "application/x-apple-diskimage")
    };

    public static void MapStaticFilesEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/files/cv", GetCv)
            .AllowAnonymous();

        webApplication.MapGet("/files/app/{platform}", GetApp)
            .AllowAnonymous();
    }

    private static IResult GetCv()
    {
        const string filename = "Daniel-Rayson-DotNet-CV.pdf";
        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(webRoot, filename);

        if (!File.Exists(filePath))
        {
            return Results.NotFound("CV file not found");
        }

        var fileStream = File.OpenRead(filePath);
        return Results.File(fileStream, "application/pdf", filename);
    }

    private static IResult GetApp(string platform)
    {
        if (!AppFiles.TryGetValue(platform.ToLowerInvariant(), out var fileInfo))
        {
            return Results.BadRequest("Invalid platform. Supported: win, linux, mac");
        }

        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(webRoot, fileInfo.Filename);

        if (!File.Exists(filePath))
        {
            return Results.NotFound($"App file not found for platform: {platform}");
        }

        var fileStream = File.OpenRead(filePath);
        return Results.File(fileStream, fileInfo.ContentType, fileInfo.Filename);
    }
}
