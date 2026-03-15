#!/bin/bash
set -x

echo "CUSTOM_MODELS: $CUSTOM_MODELS"

if ! curl -sf http://localhost:11434/api/tags > /dev/null 2>&1; then
    echo "Healthcheck: Ollama API not responding"
    exit 1
fi

RESPONSE=$(curl -s http://localhost:11434/api/tags)

MODELS_CLEANED=$(echo "$CUSTOM_MODELS" | tr -d '[]"' | tr ',' '\n')
echo "Checking models: $MODELS_CLEANED"

for model in $MODELS_CLEANED; do
    model=$(echo "$model" | tr -d '[:space:]')
    if [ -z "$model" ]; then
        continue
    fi
    
    if ! echo "$RESPONSE" | grep -q "$model"; then
        echo "Healthcheck: Model $model not found"
        exit 1
    fi
done

echo "Healthcheck: All models ready"
exit 0
