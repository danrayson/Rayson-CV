FROM ollama/ollama:latest AS ollama

FROM cgr.dev/chainguard/wolfi-base

RUN apk add --no-cache libstdc++ curl

COPY --from=ollama /usr/bin/ollama /usr/bin/ollama
COPY --from=ollama /usr/lib/ollama/libggml-cpu-* /usr/lib/ollama/
COPY --from=ollama /usr/lib/ollama/libggml-base.so /usr/lib/ollama/libggml-base.so

COPY ollama-startup.sh /ollama-startup.sh
RUN chmod +x /ollama-startup.sh

ENTRYPOINT ["/bin/sh", "/ollama-startup.sh"]
