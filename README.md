# BackgroundTasks

A API that queues long-running tasks for asynchronous background worker processing, implementing robust concurrency and ACK control.

Built with **.NET 10** and **RabbitMQ**, this solution demonstrates how to offload time-consuming work (e.g., importing products from an external API) from a web request into a durable message queue, processed independently by a worker service.

## Architecture

The solution is split into four projects:

| Project | Type | Responsibility |
|---|---|---|
| `BackgroundTasks.Api` | ASP.NET Core Web API | Accepts import requests and publishes messages to RabbitMQ |
| `BackgroundTasks.Worker` | Worker Service (`BackgroundService`) | Consumes messages from the queue and executes the long-running task |
| `BackgroundTasks.Infrastructure` | Class Library | RabbitMQ messaging (publisher + settings) shared across services |
| `BackgroundTasks.Contracts` | Class Library | Message contracts ("Task envelope") shared between API and Worker |

```
┌─────────┐   POST /api/imports   ┌──────────────┐   publish    ┌──────────────────┐
│ Client  │ ─────────────────────▶│  Api         │─────────────▶│ RabbitMQ         │
└─────────┘                       └──────────────┘              │ products.import  │
                                                                └────────┬─────────┘
                                                                         │ consume (manual ACK)
                                                                 ┌───────▼─────────┐
                                                                 │ Worker          │
                                                                 │ (DummyJson API) │
                                                                 └─────────────────┘
```

## How it works

1. The client calls `POST /api/imports` with the number of products to import.
2. The **Api** creates an `ImportProductMessage` (with a `TaskId`) and publishes it to the durable `products.import` queue, returning `202 Accepted` immediately.
3. The **Worker** consumes the message from the queue:
   - Calls the [DummyJSON](https://dummyjson.com) API to fetch products.
   - Prints each product to the console.
4. **ACK control:** the consumer runs with `autoAck: false`.
   - On success, the message is acknowledged via `BasicAckAsync` and removed from the queue.
   - On failure, the message is rejected with `BasicNackAsync` + `requeue: true`, so it is retried.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker / Docker Compose

### 1. Start RabbitMQ

The provided `compose.yaml` spins up RabbitMQ (with the management UI) on ports:

- `5672` — AMQP
- `15672` — Management UI (http://localhost:15672)

Credentials are taken from `RABBITMQ_USER` / `RABBITMQ_PASS`, so set them via environment variables or a `.env` file next to `compose.yaml`:

```bash
printf 'RABBITMQ_USER=app\nRABBITMQ_PASS=devpassword\n' > .env
docker compose up -d
```

### 2. Run the services

Open two terminals (or run both via the IDE):

```bash
# Terminal 1 – API
dotnet run --project BackgroundTasks.Api

# Terminal 2 – Worker
dotnet run --project BackgroundTasks.Worker
```

### 3. Submit a task

```bash
curl -X POST http://localhost:<api-port>/api/imports \
  -H "Content-Type: application/json" \
  -d '{ "limit": 10 }'
```

Example response:

```json
{
  "taskId": "8a3f...",
  "status": "Import requested."
}
```

In development the API project also exposes the [OpenAPI](https://aka.ms/aspnet/openapi) document at `/openapi/v1.json`.

## Configuration

Both the Api and the Worker read RabbitMQ settings from the `RabbitMQ` section of `appsettings.json` (see `RabbitMqSettings`):

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "app",
    "Password": "devpassword"
  }
}
```

## API reference

### `POST /api/imports`

Starts a product import task.

| Field | Type | Description |
|---|---|---|
| `limit` | `int` | Number of products to fetch from DummyJSON |

**Responses**

- `202 Accepted` — task queued, returns the generated `TaskId`.
