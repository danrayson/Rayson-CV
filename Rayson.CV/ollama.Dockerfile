FROM ollama/ollama:latest
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY ollama-startup.sh /ollama-startup.sh
RUN chmod +x /ollama-startup.sh
ENTRYPOINT ["/bin/bash", "/ollama-startup.sh"]
