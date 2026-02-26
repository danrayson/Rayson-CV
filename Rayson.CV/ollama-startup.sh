#!/bin/bash
set -e

# Start Ollama server in background
export OLLAMA_HOST=0.0.0.0:11434
ollama serve &

# Wait for server to be ready
echo "Waiting for Ollama server..."
until curl -s http://localhost:11434/api/tags > /dev/null 2>&1; do
    sleep 1
done

# Pull tinyllama model if not already present
if ! ollama list | grep -q "^tinyllama"; then
    echo "Pulling tinyllama model..."
    ollama pull tinyllama
else
    echo "tinyllama model already present, skipping pull"
fi

echo "Ollama is ready with tinyllama!"

# Keep container running
wait
