#!/bin/bash
set -e

# Start Ollama server in background
ollama serve &

# Wait for server to be ready
echo "Waiting for Ollama server..."
until curl -s http://localhost:11434/api/tags > /dev/null 2>&1; do
    sleep 1
done

# Pull tinyllama model if not already present
echo "Pulling tinyllama model..."
ollama pull tinyllama

echo "Ollama is ready with tinyllama!"

# Keep container running
wait
