using Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Endpoints.Files;

public static class CvEndpoints
{
    public static void MapCvEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/files/cv", GetCv)
            .AllowAnonymous();
    }

    private static async Task<IResult> GetCv(
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IOptions<BlobSettings> settings)
    {
        var blobUrl = $"{settings.Value.DownloadUrl}/$web/CV-September-2024.pdf";
        
        try
        {
            var httpClient = httpClientFactory.CreateClient("BlobStorage");
            var response = await httpClient.GetAsync(blobUrl);

            if (!response.IsSuccessStatusCode)
            {
                return Results.NotFound("Failed to fetch CV from storage");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return Results.File(stream, "application/pdf", "CV-September-2024.pdf");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error fetching CV: {ex.Message}");
        }
    }
}
