using Application.Chatbot;
using Domain;

namespace Infrastructure.Chatbot;

public class CvProvider : ICvProvider
{
    private const string ResourceName = "Domain.Resources.cv.md";

    public string GetCvContent()
    {
        var domainAssembly = typeof(Entity).Assembly;
        
        using var stream = domainAssembly.GetManifestResourceStream(ResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"CV resource not found: {ResourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
