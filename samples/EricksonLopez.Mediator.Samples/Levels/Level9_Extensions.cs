// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.AspNetCore;
using EricksonLopez.Mediator.Caching;
using EricksonLopez.Mediator.FluentValidation;
using EricksonLopez.Mediator.HealthChecks;
using EricksonLopez.Mediator.OpenTelemetry;
using EricksonLopez.Mediator.Polly;
using EricksonLopez.Mediator.Testing;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;

namespace Sample.Levels.Level9_Extensions;

// --- Extension Demonstration DTOs ---

/// <summary>Represents a query to check the health status of a named service.</summary>
/// <param name="ServiceName">The name of the service to verify.</param>
public sealed record CheckStatusQuery(string ServiceName) : IQuery<string>;

/// <summary>Handles status checks for <see cref="CheckStatusQuery"/>.</summary>
public sealed class CheckStatusQueryHandler : IQueryHandler<CheckStatusQuery, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(CheckStatusQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"Service '{query.ServiceName}' is Active");
    }
}

/// <summary>Represents a command to record an audit action.</summary>
/// <param name="Action">The audit action name to record.</param>
public sealed record TriggerAuditCommand(string Action) : ICommand<bool>;

/// <summary>Handles audit recording for <see cref="TriggerAuditCommand"/>.</summary>
public sealed class TriggerAuditCommandHandler : ICommandHandler<TriggerAuditCommand, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(TriggerAuditCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(true);
    }
}

/// <summary>Defines validation rules for <see cref="TriggerAuditCommand"/>.</summary>
public sealed class TriggerAuditCommandValidator : AbstractValidator<TriggerAuditCommand>
{
    /// <summary>Initializes a new instance of the <see cref="TriggerAuditCommandValidator"/> class.</summary>
    public TriggerAuditCommandValidator()
    {
        RuleFor(x => x.Action).NotEmpty().WithMessage("Action cannot be empty.");
    }
}

// --- Caching Demonstration DTOs ---

/// <summary>Represents a cacheable query for catalog items by category.</summary>
/// <param name="Category">The category of catalog items to retrieve.</param>
[Cacheable(120)]
public sealed record GetCachedCatalogQuery(string Category) : IQuery<string>, ICacheableRequest
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:{Category}";

    /// <inheritdoc/>
    public TimeSpan? Expiration => TimeSpan.FromSeconds(120);
}

/// <summary>Handles cached catalog retrieval for <see cref="GetCachedCatalogQuery"/>.</summary>
public sealed class GetCachedCatalogQueryHandler : IQueryHandler<GetCachedCatalogQuery, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(GetCachedCatalogQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"Catalog items for '{query.Category}'");
    }
}

/// <summary>Represents a command to invalidate cached catalog entries for a category.</summary>
/// <param name="Category">The category whose cache entries will be invalidated.</param>
public sealed record InvalidateCatalogCommand(string Category) : ICommand<bool>, IInvalidateCacheRequest
{
    /// <inheritdoc/>
    public string CachePrefix => $"catalog:{Category}";
}

/// <summary>Handles cache invalidation for <see cref="InvalidateCatalogCommand"/>.</summary>
public sealed class InvalidateCatalogCommandHandler : ICommandHandler<InvalidateCatalogCommand, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(InvalidateCatalogCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(true);
    }
}

// --- Polly Resilience Demonstration DTOs ---

/// <summary>Represents a sync command executed through a resilience pipeline.</summary>
/// <param name="Payload">The payload string to synchronize.</param>
#pragma warning disable CS0618
[UseResiliencePipeline("Default")]
public sealed record ResilientSyncCommand(string Payload) : ICommand<string>;
#pragma warning restore CS0618

/// <summary>Handles resilient synchronization for <see cref="ResilientSyncCommand"/>.</summary>
public sealed class ResilientSyncCommandHandler : ICommandHandler<ResilientSyncCommand, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(ResilientSyncCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"SYNCED:{command.Payload}");
    }
}

/// <summary>
/// Demonstrates ecosystem extensions including OpenTelemetry, ASP.NET Core, Caching, Polly, and Health Checks.
/// </summary>
public static class Demo
{
    /// <summary>Executes the Level 9 ecosystem extensions demonstration.</summary>
    /// <returns>A task representing the asynchronous demonstration operation.</returns>
    public static async Task RunAsync()
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 9: ECOSYSTEM EXTENSIONS (TELEMETRY, TESTING & WEB)");
        Console.WriteLine("================================================================================");

        // 1. AddMediatorOpenTelemetry — DI registration (compiled code)
        Console.WriteLine("1. AddMediatorOpenTelemetry() — DI configuration in executable code:");
        var otServices = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        otServices.AddMediatorOpenTelemetry(options =>
        {
            options.ActivitySourceName = "EricksonLopez.Mediator.Showcase";
            options.EnrichActivity = (activity, req) =>
                activity.SetTag("custom.environment", "Production");
        });
        var otProvider = otServices.BuildServiceProvider();
        var otelOptions = otProvider.GetRequiredService<MediatorOpenTelemetryOptions>();
        Console.WriteLine($"   -> Configured ActivitySource: {otelOptions.ActivitySourceName}");
        Console.WriteLine($"   -> EnrichActivity callback registered: {otelOptions.EnrichActivity is not null}");
        Console.WriteLine();

        // 2. OpenTelemetryBehavior<TRequest,TResponse> — compiled class usage
        Console.WriteLine("2. OpenTelemetryBehavior<TRequest,TResponse> — using the class directly:");
        Console.WriteLine("   This behavior implements IPipelineBehavior<TRequest,TResponse>.");
        Console.WriteLine("   Registration pattern:");
        Console.WriteLine("   [assembly: UseGlobalBehavior(typeof(OpenTelemetryBehavior<,>), order: 0)]");
        Console.WriteLine("   Or via DI open-generic registration:");
        Console.WriteLine("   services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OpenTelemetryBehavior<,>));");
        // Instantiate with options to verify both constructors compile
        var behaviorWithDefaults = new OpenTelemetryBehavior<CheckStatusQuery, string>();
        var behaviorWithOptions = new OpenTelemetryBehavior<CheckStatusQuery, string>(otelOptions);
        Console.WriteLine($"   -> Default constructor (uses ActivitySource 'EricksonLopez.Mediator'): activated={behaviorWithDefaults is not null}");
        Console.WriteLine($"   -> Options constructor (custom ActivitySource '{otelOptions.ActivitySourceName}'): activated={behaviorWithOptions is not null}");
        Console.WriteLine();

        // 2b. MediatorMetrics — internal OpenTelemetry counters and histogram
        Console.WriteLine("2b. MediatorMetrics — automatically recorded by OpenTelemetryBehavior:");
        Console.WriteLine("   MediatorMetrics is an internal static class in EricksonLopez.Mediator.OpenTelemetry.");
        Console.WriteLine("   It is NOT part of the public API and cannot be called directly.");
        Console.WriteLine("   OpenTelemetryBehavior invokes it automatically on every handled request.");
        Console.WriteLine("   Meter name: \"EricksonLopez.Mediator\" (version 2.0.0)");
        Console.WriteLine("   Instruments recorded:");
        Console.WriteLine("     • mediator.requests.total   (Counter<long>)   — total Send() dispatches");
        Console.WriteLine("     • mediator.notifications.total (Counter<long>) — total Publish() calls");
        Console.WriteLine("     • mediator.requests.failures  (Counter<long>)  — unhandled exceptions");
        Console.WriteLine("     • mediator.request.duration   (Histogram<double>, ms) — per-request latency");
        Console.WriteLine("   Consume via OpenTelemetryBuilder.WithMetrics(m => m.AddMeter(\"EricksonLopez.Mediator\")).");
        Console.WriteLine();

        // 3. ASP.NET Core Minimal APIs — MapCommand and MapQuery
        Console.WriteLine("3. Minimal API Routing — MapCommand<,> and MapQuery<,> compiled extension methods:");
        var appBuilder = WebApplication.CreateBuilder();
        var app = appBuilder.Build();
        app.MapCommand<TriggerAuditCommand, bool>("/api/audit");
        app.MapCommand<TriggerAuditCommand, bool>("/api/audit-update", "PUT");
        app.MapQuery<CheckStatusQuery, string>("/api/status");
        Console.WriteLine("   -> MapCommand (POST & PUT) and MapQuery (GET) endpoints registered on WebApplication.");
        Console.WriteLine();

        // 3b. Ecosystem DI Extensions (Caching, Polly, FluentValidation)
        Console.WriteLine("3b. Advanced Ecosystem DI Extensions:");
        var extServices = new ServiceCollection();
        extServices.AddMediatorCaching();
        extServices.AddMediatorPolly();
        extServices.AddMediatorDefaultResiliencePipeline(builder => { });
        extServices.AddMediatorFluentValidation();
        extServices.AddMediatorFluentValidationValidator<TriggerAuditCommandValidator, TriggerAuditCommand>();
        extServices.AddMediatorFluentValidatorsFromAssembly(typeof(Demo).Assembly);
        Console.WriteLine("   -> Caching, Polly, and FluentValidation mediator extensions registered.");
        Console.WriteLine();

        // 3c. Transparent Caching & Invalidation Behavior
        Console.WriteLine("3c. Transparent Caching Pipeline (ICacheableRequest, IInvalidateCacheRequest, CacheableAttribute):");
        // ICacheableRequest.Expiration semantics:
        //   • Non-null TimeSpan  → cache the response for exactly that duration.
        //   • null              → use the IDistributedCache default TTL (no absolute expiry set).
        var cacheBehavior = new CachingPipelineBehavior<GetCachedCatalogQuery, string>(cacheProvider: null);
        var catalogQuery = new GetCachedCatalogQuery("hardware");
        var catalogRes = await cacheBehavior.Handle(catalogQuery, new DelegateNext<string>("Hardware Catalog [Cached Mock]"), CancellationToken.None);
        Console.WriteLine($"   -> Cacheable Query: Key='{catalogQuery.CacheKey}', Expiration={catalogQuery.Expiration}");
        Console.WriteLine($"   -> Expiration=null means: use IDistributedCache default TTL (no absolute expiry).");
        Console.WriteLine($"   -> CachingPipelineBehavior (pass-through, cacheProvider=null) Result: {catalogRes}");

        var invalidateCmd = new InvalidateCatalogCommand("hardware");
        var invBehavior = new CachingPipelineBehavior<InvalidateCatalogCommand, bool>(cacheProvider: null);
        var invRes = await invBehavior.Handle(invalidateCmd, new DelegateNext<bool>(true), CancellationToken.None);
        Console.WriteLine($"   -> Invalidation Command: Prefix='{invalidateCmd.CachePrefix}', Result: {invRes}");
        Console.WriteLine();

        // 3d. FluentValidation Pipeline Behavior Execution
        Console.WriteLine("3d. FluentValidation Pipeline Behavior (ValidationPipelineBehavior):");
        var validator = new TriggerAuditCommandValidator();
        var valBehavior = new ValidationPipelineBehavior<TriggerAuditCommand, bool>(
            validators: new[] { validator });

        // Valid execution
        var validAuditRes = await valBehavior.Handle(
            new TriggerAuditCommand("SystemDiagnostics"),
            new DelegateNext<bool>(true),
            CancellationToken.None);
        Console.WriteLine($"   -> Valid command passed validation: {validAuditRes}");

        // Invalid execution
        try
        {
            await valBehavior.Handle(
                new TriggerAuditCommand(""),
                new DelegateNext<bool>(true),
                CancellationToken.None);
        }
        catch (ValidationException valEx)
        {
            Console.WriteLine($"   -> Invalid command correctly intercepted by ValidationException: {valEx.Errors.First().ErrorMessage}");
        }
        Console.WriteLine();

        // 3e. Polly Resilience Pipeline Behavior Execution
        Console.WriteLine("3e. Polly Resilience Pipeline Behavior (PollyResilienceBehavior & [UseResiliencePipeline]):");
#pragma warning disable CS0618
        var pollyPipeline = new ResiliencePipelineBuilder().Build();
        var pollyBehavior = new PollyResilienceBehavior<ResilientSyncCommand, string>(
            pipelineProvider: null,
            defaultPipeline: pollyPipeline);

        var pollyRes = await pollyBehavior.Handle(
            new ResilientSyncCommand("order-batch-42"),
            new DelegateNext<string>("SYNCED:order-batch-42"),
            CancellationToken.None);
#pragma warning restore CS0618
        Console.WriteLine($"   -> Executed through PollyResilienceBehavior: {pollyRes}");
        Console.WriteLine("   -> Note: EricksonLopez.Mediator.Polly is deprecated in favor of EricksonLopez.Resilience.Mediator (ADR-036).");
        Console.WriteLine();

        // 4. AOT-Friendly Unit Testing with FakeMediator (full demonstration)
        Console.WriteLine("4. FakeMediator — AOT unit testing without dynamic mocks:");
        var fake = new FakeMediator();

        // Sync overloads (covered in Level 9 baseline)
        fake.SetupCommand<TriggerAuditCommand, bool>(cmd => true);
        fake.SetupQuery<CheckStatusQuery, string>(q => $"Mocked: {q.ServiceName} OK");

        var auditRes = await fake.Send(new TriggerAuditCommand("UserLogin"), CancellationToken.None);
        var statusRes = await fake.Send(new CheckStatusQuery("AuthService"), CancellationToken.None);

        fake.ShouldHaveReceived<TriggerAuditCommand>(c => c.Action == "UserLogin");
        fake.ShouldHaveReceived<CheckStatusQuery>();
        Console.WriteLine($"   -> Fake Command Result: {auditRes}");
        Console.WriteLine($"   -> Fake Query Result: {statusRes}");
        Console.WriteLine($"   -> ReceivedRequests.Count = {fake.ReceivedRequests.Count}");
        Console.WriteLine($"   -> ReceivedCount<TriggerAuditCommand>() = {fake.ReceivedCount<TriggerAuditCommand>()}");
        Console.WriteLine();

        // 5. Health Checks Readiness & Liveness
        Console.WriteLine("5. Mediator Health Check (MediatorHealthCheck):");
        var fakeSender = fake;
        var healthCheck = new MediatorHealthCheck(fakeSender);
        var healthResult = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Console.WriteLine($"   -> Health Status: {healthResult.Status} ({healthResult.Description})");
        Console.WriteLine();

        // 6. MediatorHealthCheck degraded state (null sender — simulates missing DI registration)
        Console.WriteLine("6. MediatorHealthCheck degraded state (no sender registered):");
        var degradedCheck = new MediatorHealthCheck(sender: null);
        var degradedContext = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("mediator", degradedCheck, HealthStatus.Degraded, null)
        };
        var degradedResult = await degradedCheck.CheckHealthAsync(degradedContext);
        Console.WriteLine($"   -> Degraded Status: {degradedResult.Status}");
        Console.WriteLine($"   -> Description: {degradedResult.Description}");

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}
