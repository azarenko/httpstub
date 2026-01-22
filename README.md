# HttpStub

A minimal ASP.NET Core application that accepts **all** HTTP requests and echoes the request data back as JSON (method, path, query, headers, body, etc.).

## Run

```bash
dotnet run
```

By default it listens on `http://localhost:5167` (or the URLs in `Properties/launchSettings.json`). Override with:

```bash
dotnet run --urls "http://localhost:5000"
```

## Example

```bash
curl "http://localhost:5167/any/path?foo=bar" -H "X-Custom: value" -d '{"hello":1}'
```

## JSON response shape

| Field         | Description                          |
|---------------|--------------------------------------|
| `method`      | HTTP method (GET, POST, …)           |
| `path`        | Path (e.g. `/any/path`)              |
| `pathBase`    | PathBase (e.g. when behind a proxy)  |
| `queryString` | Raw query string (e.g. `?foo=bar`)   |
| `query`       | Query params as `{ "key": ["v1","v2"] }` |
| `headers`     | Headers as `{ "Name": ["value"] }`   |
| `body`        | Raw request body string              |
| `scheme`      | `http` or `https`                    |
| `host`        | Host header (e.g. `localhost:5167`)  |
| `protocol`    | HTTP protocol (e.g. `HTTP/1.1`)      |
| `contentType` | `Content-Type` header                |
| `contentLength` | `Content-Length` header           |
| `remoteIp`    | Client IP                            |

## Requirements

- .NET 8.0 SDK
