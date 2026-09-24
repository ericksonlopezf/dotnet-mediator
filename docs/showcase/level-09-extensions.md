# Level 09: Official Ecosystem Extensions

`EricksonLopez.Mediator` offers specialized, first-party extension packages for ASP.NET Core Minimal APIs, Resilience, Rate Limiting, and Validation.

---

## 1. ASP.NET Core Minimal APIs (`EricksonLopez.Mediator.AspNetCore`)

Eliminate boilerplate controller code by mapping HTTP endpoints directly to mediator commands and queries:

```csharp
using EricksonLopez.Mediator.AspNetCore;

var app = builder.Build();

// Map POST /api/users directly to CreateUserCommand
app.MapCommand<CreateUserCommand, Guid>("/api/users");

// Map GET /api/users/{id} directly to GetUserByIdQuery
app.MapQuery<GetUserByIdQuery, UserDto?>("/api/users/{id}");
```

---

## 2. Polly v8 Resilience (`EricksonLopez.Mediator.Polly`)

Wrap command execution in Polly v8 resilience pipelines (retries, timeouts, circuit breakers):

```csharp
using EricksonLopez.Mediator.Polly;
using Polly;

// 1. Register a named Polly pipeline
builder.Services.AddResiliencePipeline("database-retry", pipelineBuilder =>
{
    pipelineBuilder.AddRetry(new()
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(200),
        BackoffType = DelayBackoffType.Exponential
    });
});

// 2. Attach resilience pipeline to command
[UseResiliencePipeline("database-retry")]
public sealed record SyncCustomerDataCommand(Guid CustomerId) : ICommand<bool>;
```

---

## 3. Rate Limiting (`EricksonLopez.Mediator.RateLimiting`)

Protect sensitive or resource-intensive handlers with concurrency and sliding window rate limiters:

```csharp
using System.Threading.RateLimiting;
using EricksonLopez.Mediator.RateLimiting;

// 1. Register RateLimiter and the mediator pipeline behavior
builder.Services.AddSingleton<RateLimiter>(new ConcurrencyLimiter(new()
{
    PermitLimit = 10,
    QueueLimit = 20,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
}));
builder.Services.AddMediatorRateLimiting();

// Handlers are automatically protected via RateLimitingBehavior in the pipeline
public sealed record ChargeCreditCardCommand(decimal Amount) : ICommand<PaymentReceipt>;
```

---

## 4. FluentValidation Integration (`EricksonLopez.Mediator.FluentValidation`)

Seamlessly validate requests using FluentValidation and short-circuit the pipeline without exceptions:

```csharp
using FluentValidation;
using EricksonLopez.Mediator.FluentValidation;

// 1. Register FluentValidation behavior and validators
builder.Services.AddMediatorFluentValidation();
builder.Services.AddMediatorFluentValidatorsFromAssembly(typeof(Program).Assembly);

// 2. Define standard FluentValidation validator
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```
