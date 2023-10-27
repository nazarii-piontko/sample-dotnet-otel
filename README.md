# Sample ASP.NET (.NET 7) project with OpenTelemetry integration

## OpenTelemetry

OpenTelemetry is a collection of tools, APIs, and SDKs to instrument, generate, collect, and export telemetry data (metrics, logs, and traces) to help you analyze your software’s performance and behavior.

Links:
* https://opentelemetry.io/
* https://opentelemetry.io/docs/

There is a .NET SDK that helps to integrate .NET applications with OpenTelemetry:
* https://opentelemetry.io/docs/instrumentation/net/getting-started/
* https://github.com/open-telemetry/opentelemetry-dotnet
* https://www.nuget.org/packages/OpenTelemetry/

## Goal

The goal of this project is to play around with exporting .NET service metrics to [Prometheus](https://prometheus.io/), tracing service-to-service communication with [Jaeger](https://www.jaegertracing.io/), and collecting logs with [Loki](https://grafana.com/oss/loki/) using OpenTelementry .NET SDK.

## Components diagram

![Components Diagram](Images/Diagram.png)

[Open in app.diagrams.net](https://viewer.diagrams.net/?url=https://raw.githubusercontent.com/nazarii-piontko/sample-dotnet-otel/main/Diagram.xml)

## Implementation details

Two .NET services have been implemented:

The first one is `SampleDotNetOTEL.BusinessService`. It exposes 3 endpoints:
* Get fake auto-generated weather records from the database (PostgreSQL).
* Get `"Hello World"` string
* Get `"Hello {USER_NAME}"` string for the passed username parameter. 

Also `SampleDotNetOTEL.BusinessService` has injected "faults" to simulate service errors.

And also `SampleDotNetOTEL.BusinessService` reads messages from RabbitMQ and logs them.

The second one is `SampleDotNetOTEL.ProxyService`. It exposes the same 3 endpoints as the first service but it just makes an HTTP request to `SampleDotNetOTEL.BusinessService` and forwards the response as it is.
Also, it provides the fourth endpoint to `POST` message which will be queued to RabbitMQ and later read by `SampleDotNetOTEL.BusinessService` to log it.

All services could be run with `docker-compose`.

`docker-compose.yml` contains as one of the services a dummy client called `spammer` which makes requests to 3 endpoints every half a second.

## How to run locally

* Ensure you have `Docker` installed and running.
* Ensure you have `docker-compose` installed.
* Run `docker-compose build`
* Run `docker-compose up`
* As soon as `docker-compose` starts services they should be available via HTTP. Here are some links:
  * Grafana pre-build dashboard should be accessible via http://localhost:3000/d/KdDACDp4z/asp-net-otel-metrics
  * Jaeger should be accessible via http://localhost:16686/search
  * Prometheus should be accessible via http://localhost:9090/graph
  * Proxy service should be accessible via http://localhost:8080/hello

## Screenshots

### Jaeger

#### Jaeger trace

##### Jaeger trace for HTTP
![Jaeger trace for HTTP](Images/JaegerTraceHTTP.png)

##### Jaeger trace for RabbitMQ
![Jaeger trace for RabbitMQ](Images/JaegerTraceRabbitMQ.png)

#### Jaeger monitor

![Jaeger monitor](Images/JaegerMonitor.png)

### Grafana

Dashboard: https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics

![Grafana dashboard part 1](Images/GrafanaOTELMetrics1.png)

![Grafana dashboard part 2](Images/GrafanaOTELMetrics2.png)
