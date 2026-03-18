# Rayson.Ollama

.NET client library for Ollama with Docker support.

## Installation

```bash
dotnet add package Rayson.Ollama
```

> **Note:** This package creates 5 files in your project when you build, these are your ollama configuration files.

## QuickStart

1. Install the NuGet package and Build your project
2. Move `ollama.Dockerfile` and `*.sh` scripts to suitable location
3. Merge `ollama.docker-compose.yml` into your compose
4. Update `ollama>build>context` to files location
5. Configure `CUSTOM_MODELS` with your desired models
6. Configure `Ollama__Url` for your deployment scenario (see Configuration>Docker section below)
7. Optionally add GPU support, see comment in compose file

### Usage:

Using .NET's ServiceCollection for dependency injection:

```csharp
using Rayson.Ollama.Extensions
services.AddOllama();
```

```csharp
using Rayson.Ollama
var ollama = sp.GetRequiredService<IOllamaService>();

// Generate text (non-streaming)
var result = await ollama.GenerateAsync("Hello", "smollm:135m");
Console.WriteLine(result.Response);

// Generate with streaming
await foreach (var chunk in ollama.GenerateStreamAsync("Hello", "smollm:135m"))
{
    Console.Write(chunk.Response);
}

// Chat (non-streaming)
var chatResult = await ollama.ChatAsync(
    [new() { Role = "user", Content = "Hello" }], 
    "smollm:135m");
Console.WriteLine(chatResult.Message?.Content);

// Chat with streaming
await foreach (var chunk in ollama.ChatStreamAsync(
    [new() { Role = "user", Content = "Hello" }], 
    "smollm:135m"))
{
    Console.Write(chunk.Message?.Content);
}

// ## Prerequisites

### NVIDIA GPU Support

If using an NVIDIA GPU, install the [NVIDIA Container Toolkit](https://docs.nvidia.com/datacenter/cloud-native/container-toolkit/latest/install-guide.html):

```bash
# Configure the repository
curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey \
    | sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg
curl -fsSL https://nvidia.github.io/libnvidia-container/stable/deb/nvidia-container-toolkit.list \
    | sed 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' \
    | sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list
sudo apt-get update

# Install the toolkit
sudo apt-get install -y nvidia-container-toolkit

# Configure Docker
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker
```

After installation, enable NVIDIA GPU support in `ollama.docker-compose.yml` by uncommenting the `deploy` section.

## Docker Setup

### Running Ollama

```bash
docker compose up --build
```

> **Note:** By default, Ollama models are persisted in a Docker volume (`ollama_data`). This preserves your downloaded models across container restarts.

### Healthcheck

The docker-compose includes a healthcheck that polls the Ollama API every 10 seconds. You can check the health status with:

```bash
docker compose ps
```

### Using Multiple Models

By default, the container starts with `smollm:135m`. To configure models, edit the `CUSTOM_MODELS` environment variable in `ollama.docker-compose.yml`:

```yaml
environment:
  - CUSTOM_MODELS=["smollm:135m"]
```

The `CUSTOM_MODELS` environment variable accepts a JSON array of model names. The entrypoint script will check if each model exists and pull it only if missing.

## Configuration

### Docker

#### Ollama container

| Environment Variable | Default | Description |
|---------------------|---------|-------------|
| `CUSTOM_MODELS` | `["smollm:135m"]` | Models to check/pull on startup |

> **Note:**  The `OLLAMA_HOST` in the Ollama container is hardcoded to `0.0.0.0` so connections from other containers are accepted.  This could raise security issues for your environment.

#### Consumer container

| Environment Variable | Default | Description |
|---------------------|---------|-------------|
| `Ollama__Url` | `http://ollama:11434` | URL for calling containers configuration to reach Ollama API (Docker internal network) |

### Connecting to Ollama

The `Ollama__Url` environment variable specifies the address other containers use to reach Ollama. Set this based on your deployment scenario:

- **Container-to-container** (recommended): Use `http://ollama:11434` (Docker internal network)
- **Host machine access**: Use `http://localhost:11434` and add `network_mode: host` to your compose:

```yaml
services:
  your-app:
    network_mode: host
```

### .NET Client

Configure the Ollama URL in your .NET application to match the `Ollama__Url` value from your docker-compose:

**Via docker-compose.yml:**
```yaml
services:
  webapp:
    build: .
    image: my-example-app:latest
    environment:
      - Ollama__Url=http://ollama:11434
```

The URL should match the `Ollama__Url` value in your docker-compose.

## API Reference

### IOllamaService

```csharp
// Generate text (non-streaming)
Task<GenerateResponse> GenerateAsync(string prompt, string model, GenerateOptions? options = null, CancellationToken ct = default)

// Generate text with streaming
IAsyncEnumerable<GenerateResponse> GenerateStreamAsync(string prompt, string model, GenerateOptions? options = null, CancellationToken ct = default)

// Chat (non-streaming)
Task<ChatResponse> ChatAsync(IList<OllamaMessage> messages, string model, ChatOptions? options = null, CancellationToken ct = default)

// Chat with streaming
IAsyncEnumerable<ChatResponse> ChatStreamAsync(IList<OllamaMessage> messages, string model, ChatOptions? options = null, CancellationToken ct = default)

// Generate embeddings
Task<EmbeddingsResponse> EmbedAsync(string input, string model, CancellationToken ct = default)

// List available models
Task<IEnumerable<ModelInfo>> ListModelsAsync(CancellationToken ct = default)

// Pull a model
Task PullModelAsync(string model, IProgress<string>? progress = null, CancellationToken ct = default)

// Delete a model
Task DeleteModelAsync(string model, CancellationToken ct = default)
```

### Options

```csharp
// Options for Generate/GenerateStream
public class GenerateOptions
{
    public string? Model { get; set; }
    public string? Prompt { get; set; }
    public string? System { get; set; }
    public string? Template { get; set; }
    public string? Context { get; set; }
    public bool Raw { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
    public double? RepeatPenalty { get; set; }
    public int? NumPredict { get; set; }
    public IList<string>? Stop { get; set; }
}

// Options for Chat/ChatStream
public class ChatOptions
{
    public string? Model { get; set; }
    public string? System { get; set; }
    public IList<OllamaMessage>? Messages { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
    public double? RepeatPenalty { get; set; }
    public int? NumPredict { get; set; }
    public IList<string>? Stop { get; set; }
}
```

### Response Types

```csharp
public class OllamaMessage
{
    public string? Role { get; set; }
    public string? Content { get; set; }
}

public class OllamaResponse
{
    public string? Model { get; set; }
    public bool Done { get; set; }
    public string? DoneReason { get; set; }
    public long? TotalDuration { get; set; }
    public long? LoadDuration { get; set; }
    public int? PromptEvalCount { get; set; }
    public int? EvalCount { get; set; }
    public long? EvalDuration { get; set; }
}

public class GenerateResponse : OllamaResponse
{
    public string? Response { get; set; }
}

public class ChatResponse : OllamaResponse
{
    public OllamaMessage? Message { get; set; }
}

public record ModelInfo(string Name, string ModifiedAt, long Size);

public record EmbeddingsResponse(IReadOnlyList<double> Embedding);
```
