#!/bin/sh
set -e

# Start Ollama server in background
export OLLAMA_HOST=0.0.0.0:11434
ollama serve &

# Wait for server to be ready
echo "Waiting for Ollama server..."
until curl -s http://localhost:11434/api/tags > /dev/null 2>&1; do
    sleep 1
done

# Pull smollm2:135m model if not already present
if ! ollama list | grep -q "^smollm2:135m"; then
    echo "Pulling smollm2:135m model..."
    ollama pull smollm2:135m
else
    echo "smollm2:135m model already present, skipping pull"
fi

echo "Ollama is ready with smollm2:135m!"

# Keep container running
wait
