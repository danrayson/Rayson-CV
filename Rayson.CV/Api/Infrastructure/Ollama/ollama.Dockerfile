FROM ollama/ollama:latest

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

ENV OLLAMA_HOST=0.0.0.0

COPY ollama.entrypoint.sh /
COPY ollama.healthcheck.sh /

RUN chmod +x /ollama.entrypoint.sh /ollama.healthcheck.sh

EXPOSE 11434

ENTRYPOINT ["/ollama.entrypoint.sh"]