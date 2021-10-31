# Status Page Backend

The Backend of Status Page is ASP&#46;NET Server that provides REST API to data access and business logic. Data are stored in Mongo database. The server is distributed as Docker image. There are also several NuGet packages that simplify server usage and testing.

![example workflow](https://github.com/status-page-net/backend/actions/workflows/build.yml/badge.svg)

## Overview

The repository contains .NET projects that are described below in brief.

### StatusPage.Api

The most important project that defines Backend API models and interfaces. The interfaces are divided on two groups:
- Data Access Layer (DAL) interfaces.
- Business Logic Layer (BLL) interfaces.

Other projects in the solution implement these interfaces:

- `StatusPage.BLL`
- `StatusPage.Client`
- `StatusPage.Mock`
- `StatusPage.MongoDB`

The project type is .NET Class Library. Distributed as NuGet package.

### StatusPage.MongoDB

The project implements DAL interfaces for Mongo database. The actual work with database is hidden behind C# interfaces.

The project type is .NET Class Library.

### StatusPage.BLL

The project implements BLL interfaces of `StatusPage.Api`. This implementation relies on DAL interfaces that are used via Dependency Injection mechanic.

The project type is .NET Class Library.

### StatusPage.Server

ASP&#46;NET Server that exposes BLL via REST API. The Server is the part of HTTP Transport that converts HTTP requests to calls of BLL interfaces. `StatusPage.MongoDB` is used as DAL implementation.

```
┌───────────────────┐    ┌────────────────┐    ┌────────────────────┐
│ StatusPage.Server │ => │ StatusPage.BLL │ => │ StatusPage.MongoDB │
└───────────────────┘    └────────────────┘    └────────────────────┘
```

The project type is ASP&#46;NET Core Web API. Distributed as Docker image.

### StatusPage.Client

The project implements BLL interfaces of `StatusPage.Api` to access Server REST API. The Client is the part of HTTP Transport that converts calls of BLL interfaces to HTTP requests.

```
┌───────────────────┐                    ┌───────────────────┐
│ StatusPage.Client │ => HTTP request => │ StatusPage.Server │
└───────────────────┘                    └───────────────────┘
```

The project type is .NET Class Library. Distributed as NuGet package.

### StatusPage.Mock

The project implements DAL interfaces of `StatusPage.Api` for testing purposes only. Data are stored in-memory. The Mock allows to replace the real out-process database with in-process clone to speed up tests, make them fast and reproducible.

Integration tests give a guarantee that Mock and MongoDB DAL implementations have the same behaviour for small amount of data.

The project type is .NET Class Library. Distributed as NuGet package.

### StatusPage.UnitTest

Unit tests. The `StatusPage.Mock` is used to test BLL implementation.

The project type is MSTest.

### StatusPage.IntegrationTest

Integration tests. The same test set is running for three different combinations of projects:
1. `StatusPage.BLL` vs. `StatusPage.MongoDB`. Ensures that BLL works against real Mongo database. The local MongoDB server is required.
2. `StatusPage.BLL` vs. `StatusPage.Mock`. Ensures that Mock behave just like real database.
3. `StatusPage.Client` vs. `StatusPage.Server` + `StatusPage.Mock`. Ensures that Client works with the Server.

The project type is MSTest.

## Local build

The development and debugging of every project in the repository is possible locally, without any external dependencies.

Use standard .NET commands to build and run tests on any platform:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```