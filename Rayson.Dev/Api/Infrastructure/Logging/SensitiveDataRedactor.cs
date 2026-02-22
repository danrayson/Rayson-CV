using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Logging;

public static class SensitiveDataRedactor
{
    private const string RedactedValue = "[REDACTED]";

    private static readonly string[] SensitiveFieldPatterns =
    {
        "password",
        "token",
        "secret",
        "apikey",
        "api_key",
        "authorization",
        "credential"
    };

    public static string Redact(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        try
        {
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode is null)
            {
                return json;
            }

            RedactNode(jsonNode);
            return jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private static void RedactNode(JsonNode node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                RedactObject(jsonObject);
                break;
            case JsonArray jsonArray:
                RedactArray(jsonArray);
                break;
        }
    }

    private static void RedactObject(JsonObject jsonObject)
    {
        var propertiesToRedact = new List<string>();

        foreach (var property in jsonObject)
        {
            if (IsSensitiveField(property.Key))
            {
                propertiesToRedact.Add(property.Key);
            }
            else if (property.Value is not null)
            {
                RedactNode(property.Value);
            }
        }

        foreach (var propertyName in propertiesToRedact)
        {
            jsonObject[propertyName] = RedactedValue;
        }
    }

    private static void RedactArray(JsonArray jsonArray)
    {
        foreach (var item in jsonArray)
        {
            if (item is not null)
            {
                RedactNode(item);
            }
        }
    }

    private static bool IsSensitiveField(string fieldName)
    {
        var lowerFieldName = fieldName.ToLowerInvariant();
        return SensitiveFieldPatterns.Any(pattern => lowerFieldName.Contains(pattern));
    }
}
