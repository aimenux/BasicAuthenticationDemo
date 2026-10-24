[![.NET](https://github.com/aimenux/BasicAuthenticationDemo/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/aimenux/BasicAuthenticationDemo/actions/workflows/ci.yml)

# 🔐 BasicAuthenticationDemo

> 🧪 **Eight ways to add HTTP Basic authentication to ASP.NET Core Web APIs.**

> [!WARNING]
> ⚠️ **This is a demo.** Basic authentication sends a username and password with **every request**. The credentials are only **base64-encoded**, which is *not* encryption. Anyone who can read the traffic can read the credentials.
>
> 🔒 **Always use it over HTTPS**, and prefer token-based authentication (OAuth 2.0 / OpenID Connect) for real systems.

---

## 📑 Table of contents

- [🎯 What is this?](#-what-is-this)
- [🔒 Why HTTPS is mandatory](#-why-https-is-mandatory)
- [🚀 Quick start](#-quick-start)
- [🧩 Examples at a glance](#-examples-at-a-glance)
- [🛡️ Security posture](#️-security-posture)
- [🧪 Testing](#-testing)
- [🏭 Going to production](#-going-to-production)
- [🗂️ Project structure](#️-project-structure)
- [🛠️ Tools](#️-tools)

---

## 🎯 What is this?

A **teaching repository** that shows how to protect ASP.NET Core Web APIs with HTTP Basic authentication, in eight variations:

| Dimension | Variants |
|-----------|----------|
| 🧱 API style | Controllers · Minimal APIs |
| 🔑 Mechanism | Middleware · Authorization filters · Endpoint filters · Custom authentication scheme |
| 📍 Scope | Global · Route group · Per action · Per route · Policy |

Each example exposes the same two endpoints:

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/movies/list` | Returns all movies |
| `GET` | `/api/movies/{movieId}` | Returns one movie (`404` if unknown) |

> ℹ️ The movie data and the demo credentials are hardcoded on purpose. This is not a real user store.

---

## 🔒 Why HTTPS is mandatory

| ❌ Over plain HTTP | ✅ Over HTTPS |
|--------------------|---------------|
| Credentials visible to anyone on the path | Credentials encrypted in transit |
| Replayable forever (no expiry) | Still replayable if leaked, so rotate passwords |
| Must be prevented in the transport layer | Enforced by this demo (see [Security posture](#️-security-posture)) |

---

## 🚀 Quick start

### 1️⃣ Prerequisites

- 🟣 [.NET SDK 10.0](https://dotnet.microsoft.com/download) (all projects target `net10.0`)
- 🔐 A trusted ASP.NET Core development certificate:

```bash
dotnet dev-certs https --trust
```

### 2️⃣ Run an example

```bash
dotnet run --project src/Example01
```

Then open **Swagger UI** (Development environment) at the HTTPS URL for that example, for example `https://localhost:5001/swagger` for Example01.

> 🔌 Each example listens on its own HTTPS port (`5001`–`5008`, see the table above), set in `appsettings.Development.json` (`Https:Port`). Examples can run side by side. Integration tests use port `5000` in memory and never bind it.

#### 🔌 How the HTTPS port is chosen

1. `Https:Port` from configuration (`appsettings.Development.json`, or an environment variable such as `Https__Port`).
2. Otherwise, the HTTPS URL from `urls`, which is populated from `ASPNETCORE_URLS`, the `--urls` argument, or the launch profile `applicationUrl`.
3. If neither is set, the app fails at startup with `No HTTPS port configured`.

### 3️⃣ Call the API

Demo credentials (defined in each example's `BasicConstants.cs`):

| Username | Password |
|----------|----------|
| `UserAbc` | `Pass123` |

```bash
# ✅ Valid credentials over HTTPS
curl -u UserAbc:Pass123 https://localhost:5001/api/movies/list

# ❌ No credentials → 401 Unauthorized
curl https://localhost:5001/api/movies/list

# ❌ Plain HTTP → connection refused (the server only listens on HTTPS)
curl http://localhost:5001/api/movies/list
```

---

## 🧩 Examples at a glance

| # | 🧱 Style | 🔑 Mechanism | 📍 Scope | ❓ Status Code | 🔌 Port |
|---|-----------|--------------|----------|----------------------------------|-------------------|
| 01 | Controllers | Middleware | Global | **401** | `5001` |
| 02 | Minimal API | Middleware | Global | **401** | `5002` |
| 03 | Controllers | Authorization filter | Global | 404 | `5003` |
| 04 | Minimal API | Endpoint filter | Per route group | 404 | `5004` |
| 05 | Controllers | Authorization filter | Per action (attribute) | 404 | `5005` |
| 06 | Minimal API | Endpoint filter | Per route | 404 | `5006` |
| 07 | Controllers | Custom authentication scheme | Per route group | 404 | `5007` |
| 08 | Minimal API | Custom authentication scheme | Per route | 404 | `5008` |

### 🔍 Reading the table

- **Middleware (01, 02)** runs for every request, so unknown routes are also protected and return `401`.
- **Filters and schemes (03–08)** run only for matched endpoints, so unknown routes return `404` as usual.
- **Protection by default:** route-group and global filters (03, 04) protect routes added later to that scope. Per-route attributes and filters (05, 06, 08) must be repeated on each new route, so a new route is public until it is protected.

### 🧭 Which one should I read first?

| If you want to learn… | Start with |
|-----------------------|------------|
| The simplest possible approach | 🥇 **01** (controllers + middleware) |
| How endpoint filters work in minimal APIs | 🥈 **04** (route group filter) |
| The ASP.NET Core way (authentication schemes) | 🥉 **07** / **08** |

---

## 🛡️ Security posture

What the demo **does** implement:

| ✅ Measure | Where | Why it matters |
|-----------|-------|----------------|
| 🔐 **HTTPS-only listener** | `WithHttpsCheck()` in each `Program.cs` | Kestrel never opens an HTTP socket, so plain-HTTP clients are refused |
| 🔢 **Configurable HTTPS port** | `Https:Port` in `appsettings.Development.json` | No port hardcoded in code; `--urls` cannot add an HTTP listener |
| ⏳ **HSTS** | `app.UseHsts()` | Browsers that have visited over HTTPS will upgrade future requests |
| 🪪 **`WWW-Authenticate: Basic`** | Every `401` response | Browsers show a login prompt, and clients can discover the scheme |
| ⏱️ **Constant-time comparison** | `IsValidUsername` / `IsValidPassword` | Both values are hashed with SHA-256 and compared with `CryptographicOperations.FixedTimeEquals`, so timing does not reveal the credentials |
| 🧮 **No short-circuit** | `!IsValidUsername(u) \| !IsValidPassword(p)` | The username check does not skip the password check, so timing does not reveal whether a username exists |

What the demo **does not** implement (on purpose, to stay focused):

- 🙈 Credential storage: passwords are hardcoded constants.
- 🚦 Rate limiting or account lockout against brute-force attacks.
- 🔁 Credential rotation or revocation.
- 🌐 Reverse proxy or load balancer configuration (`ForwardedHeaders`).
- 🧾 Audit logging of authentication failures.

---

## 🧪 Testing

Each example has its own test project (`test/ExampleNN.Tests`), built on xUnit, AwesomeAssertions (a FluentAssertions fork), and NSubstitute.

```bash
dotnet test
```

| Suite | Scope | Examples |
|-------|-------|----------|
| 🧩 **Unit** | Controllers, endpoints, middleware and handlers, with mocked repositories | All |
| 🌐 **Integration** | Real pipeline through `WebApplicationFactory`, with HTTP calls | All |

Integration tests cover:

- ✅ Valid credentials return `200` (for two routes)
- ❌ Wrong username or password return `401`
- ❌ Missing `Authorization` header returns `401`
- ❌ Malformed base64 returns `401`
- ❌ Malformed scheme (`Bearer`) returns `401`
- 🔎 Unknown movie ID returns `404`
- 🔒 HTTPS responses include `Strict-Transport-Security`
- 🚫 HTTP responses do not include `Strict-Transport-Security`

---

## 🏭 Going to production

Basic authentication is a teaching tool. For a real API, consider:

| Option | Use it when… |
|--------|--------------|
| 🎟️ **Bearer tokens (JWT)** issued by an identity provider | You need stateless APIs and centralized login |
| 🪪 **OAuth 2.0 / OpenID Connect** (Microsoft Entra ID, Auth0, Keycloak, …) | You need SSO, scopes, or third-party clients |
| 🔑 **API keys** for trusted machine-to-machine calls | You need simple server-to-server access (still HTTPS only) |
| 🧱 **Mutual TLS** | You control both client and server and need strong client identity |

If you must keep Basic authentication, at least:

1. 🔐 Serve it only over HTTPS, terminated by a reverse proxy or the server itself.
2. 🗝️ Store hashed passwords (for example with ASP.NET Core Identity or a password hasher), never constants.
3. 🚦 Add rate limiting and lockout.
4. 🧾 Log failures without logging credentials.

---

## 🗂️ Project structure

```text
BasicAuthenticationDemo/
├── 📁 src/
│   ├── 📁 Example01/                   # Controllers + middleware
│   ├── 📁 Example02/                   # Minimal API + middleware
│   ├── 📁 Example03/                   # Controllers + global authorization filter
│   ├── 📁 Example04/                   # Minimal API + route group endpoint filter
│   ├── 📁 Example05/                   # Controllers + per-action authorization filter
│   ├── 📁 Example06/                   # Minimal API + per-route endpoint filter
│   ├── 📁 Example07/                   # Controllers + custom authentication scheme
│   └── 📁 Example08/                   # Minimal API + custom authentication scheme
├── 📁 test/                            # One xUnit project per example
├── 📄 Directory.Build.props            # Shared build settings
├── 📄 Directory.Packages.props         # Central package versions
└── 📄 global.json                      # Pinned .NET SDK
```

Each example is self-contained: its own `Program.cs`, `Extensions.cs` (`WithHttpsCheck`), `appsettings.*.json`, and `Presentation/Authentication/` folder.

---

## 🛠️ Tools

| Tool | Purpose |
|------|---------|
| 🟣 **.NET 10 / ASP.NET Core** | Web API, Kestrel, authentication and authorization |
| 📘 **Swagger / Swashbuckle** | Interactive API docs (Development only) |
| 🧪 **xUnit** | Test framework |
| ✨ **AwesomeAssertions** | Readable assertions |
| 🎭 **NSubstitute** | Mocking repositories |
| 🔁 **GitHub Actions** | CI build and test |

---

<p align="center">
  <sub>Made for learning 📚 · Not for production 🚫 · Always use HTTPS 🔒</sub>
</p>
