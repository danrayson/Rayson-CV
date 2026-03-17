#!/bin/bash
set -e

if [ -z "$OLLAMA_HOST" ]; then
    echo "ERROR: OLLAMA_HOST environment variable is required"
    exit 1
fi

if [ -z "$CUSTOM_MODELS" ]; then
    echo "ERROR: CUSTOM_MODELS environment variable is required"
    exit 1
fi

# This is done in the image build already
# echo "Installing curl..."
# apt-get update && apt-get install -y curl

echo "Starting Ollama Service..."
ollama serve &

echo "Waiting for Ollama to be ready..."
until curl -s http://localhost:11434/api/tags > /dev/null 2>&1; do
    sleep 1
done

MODELS=${CUSTOM_MODELS}
MODELS_CLEANED=$(echo "$MODELS" | tr -d '[]"' | tr ',' '\n')

for model in $MODELS_CLEANED; do
    model=$(echo "$model" | tr -d '[:space:]')
    if [ -z "$model" ]; then
        continue
    fi
    
    echo "Checking model: $model"
    if ollama list | grep -q "$model"; then
        echo "Model $model already exists"
    else
        echo "Pulling model: $model"
        ollama pull "$model"
    fi
done

echo "Finished pulling models"
wait