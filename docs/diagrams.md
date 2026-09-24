# Comprehensive Architectural Diagrams — EricksonLopez.Mediator

This document contains the official Mermaid diagrams documenting the verified architecture and runtime topology of `EricksonLopez.Mediator`.

---

## 1. Overall System Architecture

```mermaid
graph TB
    subgraph ConsumerApps["Consumer Applications"]
        WebAPI["ASP.NET Core Web APIs (Minimal APIs / Controllers)"]
        Serverless["Serverless Functions / AWS Lambda / Cloudflare Workers"]
        Workers["Background Workers / Hosted Services"]
    end

    subgraph CoreAbstractions["Core Abstractions (EricksonLopez.Mediator)"]
        IMediator["IMediator / ISender / IPublisher"]
        Contracts["ICommand<T> / IQuery<T> / IStreamRequest<T> / INotification / IRequest<T>"]
        Handlers["ICommandHandler / IQueryHandler / IStreamRequestHandler / INotificationHandler / IRequestHandler"]
        PipelineInterfaces["IPipelineBehavior<TRequest, TResponse> / INotificationBehavior<TNotification>"]
        StructContinuations["struct INext<TResponse> / struct INext"]
        StaticMed["StaticMediator (Zero-DI Engine)"]
        UnitType["Unit (Canonical Void Representation)"]
        ValidationAttrs["[ValidateRequest], [ValidateNotNull], [ValidateNotEmpty], [ValidateRange], [ValidateLength], [ValidateRegex]"]
        ConfigAttrs["[UseBehavior], [UseGlobalBehavior], [ServiceLifetime], [PublishStrategy], [DiscoverHandlers]"]
        HealthChecks["MediatorHealthCheck / MediatorHealthCheckExtensions"]
    end

    subgraph CompileTimeEngine["Compile-Time Engine (Roslyn Source Generator)"]
        Generator["EricksonLopez.Mediator.Generator (netstandard2.0)"]
        StaticSwitch["GeneratedMediator (Static Type-Switch Dispatch)"]
        DIExtensions["GeneratedMediatorExtensions (AddEricksonLopezMediator)"]
        Diagnostics["IDE Diagnostics (ELM001 - ELM011)"]
    end

    subgraph EcosystemPackages["Infrastructure & Extension Packages"]
        AspNetCore["EricksonLopez.Mediator.AspNetCore (MapCommand, MapQuery)"]
        Caching["EricksonLopez.Mediator.Caching (CachingPipelineBehavior, ICacheableRequest)"]
        FluentVal["EricksonLopez.Mediator.FluentValidation (ValidationPipelineBehavior)"]
        OTel["EricksonLopez.Mediator.OpenTelemetry (OpenTelemetryBehavior, MediatorMetrics)"]
        Polly["EricksonLopez.Mediator.Polly (PollyResilienceBehavior - Deprecated ADR-036)"]
        RateLim["EricksonLopez.Mediator.RateLimiting (RateLimitingBehavior)"]
        Result["EricksonLopez.Mediator.Result (IResultFactory<TResponse>)"]
        Testing["EricksonLopez.Mediator.Testing (FakeMediator, DelegateNext<T>)"]
    end

    ConsumerApps --> CoreAbstractions
    ConsumerApps --> EcosystemPackages
    Generator -.->|Inspects code at compile-time| CoreAbstractions
    Generator -.->|Emits C# source code| ConsumerApps
    EcosystemPackages --> CoreAbstractions
```

---

## 2. Main System Execution Flow

```mermaid
flowchart TD
    Start([Start: Inbound Request]) --> InboundDecision{Invocation Source}

    InboundDecision -->|HTTP Route| MinimalApi["MapCommand / MapQuery Endpoint"]
    InboundDecision -->|In-Process Service| DISender["ISender.SendCommand / SendQuery / Send"]
    InboundDecision -->|Serverless / Zero-DI| StaticDispatch["StaticMediator.SendCommand / SendQuery"]

    MinimalApi --> DISender
    DISender --> RoslynSwitch["GeneratedMediator (Compile-Time Switch Table)"]

    RoslynSwitch --> PipelineDecision{Configured Behaviors?}
    PipelineDecision -->|Yes| PipelineTraversal["Struct INext<TResponse> Traversal (OTel, RateLimit, Caching, Validation)"]
    PipelineDecision -->|No| InvokeHandler["Direct Handler Invocation"]

    PipelineTraversal --> ShortCircuit{Short-Circuit / Error?}
    ShortCircuit -->|Yes: Cache Hit| ReturnCache["Return Cached Value"]
    ShortCircuit -->|Yes: Error / Validation| ReturnResultFactory["Return Failure via IResultFactory or Throw Exception"]
    ShortCircuit -->|No| InvokeHandler

    InvokeHandler --> ExecuteBusiness["ICommandHandler / IQueryHandler ExecuteAsync"]
    ExecuteBusiness --> HandlerReturn["ValueTask<TResponse>"]

    HandlerReturn --> PostProcessPipeline["Return via Pipeline (Cache Set, OTel Activity.SetStatus, StopWatch)"]
    PostProcessPipeline --> FinalResponse([Response Delivered to Caller])
    ReturnCache --> FinalResponse
    ReturnResultFactory --> FinalResponse

    StaticDispatch --> StaticTable["ConcurrentDictionary Lookup"]
    StaticTable --> ExecuteBusiness
```

---

## 3. Sequence Diagram (Synchronous Dispatch & Reactive Streaming)

```mermaid
sequenceDiagram
    autonumber
    actor Caller as Inbound Caller / Endpoint
    participant Sender as ISender
    participant GenMediator as GeneratedMediator
    participant Behaviors as Pipeline Behaviors (Struct INext)
    participant Handler as ICommandHandler / IQueryHandler
    participant StreamHandler as IStreamRequestHandler

    rect rgb(240, 248, 255)
        note over Caller, Handler: Flow 1: Standard Command or Query Dispatch
        Caller->>Sender: SendCommand<TCommand, TResponse>(command, ct)
        Sender->>GenMediator: SendCommand(command, ct)
        GenMediator->>Behaviors: Handle(command, next1, ct)
        Behaviors->>Behaviors: next1.InvokeAsync() [struct INext]
        Behaviors->>Handler: Handle(command, ct)
        Handler-->>Behaviors: ValueTask<TResponse>
        Behaviors-->>GenMediator: ValueTask<TResponse>
        GenMediator-->>Sender: ValueTask<TResponse>
        Sender-->>Caller: TResponse
    end

    rect rgb(255, 250, 240)
        note over Caller, StreamHandler: Flow 2: Reactive Asynchronous Streaming
        Caller->>Sender: CreateStream<TResponse>(streamRequest, ct)
        Sender->>GenMediator: CreateStream(streamRequest, ct)
        GenMediator->>StreamHandler: Handle(streamRequest, ct)
        StreamHandler-->>Caller: IAsyncEnumerable<TResponse>
        loop For each available element
            Caller->>StreamHandler: MoveNextAsync()
            StreamHandler-->>Caller: yield return TResponse
        end
    end
```

---

## 4. State Machine Diagram (Request & Notification Lifecycles)

```mermaid
stateDiagram-v2
    state "CQRS Request Lifecycle" as RequestLifecycle {
        [*] --> Created: Record/Class Instantiation
        Created --> Dispatched: Send / SendCommand / SendQuery
        Dispatched --> PipelineEvaluating: Struct Pipeline Traversal
        PipelineEvaluating --> ShortCircuited: Validation Failure (IResultFactory) / Cache Hit
        PipelineEvaluating --> HandlerExecuting: Continuations Resolved
        HandlerExecuting --> HandledSuccess: ValueTask Completed Successfully
        HandlerExecuting --> HandledFaulted: Unhandled Domain/Business Exception
        ShortCircuited --> Completed: Non-exceptional Return
        HandledSuccess --> Completed: Metrics Recorded & Response Returned
        HandledFaulted --> TerminatedWithException: Activity.SetStatus(Error) & Re-throw
        Completed --> [*]
        TerminatedWithException --> [*]
    }

    state "Notification Lifecycle" as NotificationLifecycle {
        [*] --> EventPublished: IPublisher.Publish(notification)
        EventPublished --> StrategySelection: [PublishStrategy] Evaluation
        StrategySelection --> SequentialMode: Sequential (Default)
        StrategySelection --> ParallelMode: Parallel (Task.WhenAll)
        StrategySelection --> AggregateMode: SequentialAggregateExceptions

        SequentialMode --> SequentialLoop: Execute Handler N
        SequentialLoop --> SequentialFailed: Exception Thrown (Immediate Halt)
        SequentialLoop --> AllHandlersDone: All Executed Successfully

        ParallelMode --> ConcurrentExecution: Task.WhenAll(Handlers)
        ConcurrentExecution --> AllHandlersDone

        AggregateMode --> AggregateLoop: Execute Handler N (Try/Catch)
        AggregateLoop --> AggregateCheck: Errors Captured?
        AggregateCheck --> ThrowAggregate: Throw NotificationHandlerAggregateException
        AggregateCheck --> AllHandlersDone: Zero Exceptions

        AllHandlersDone --> [*]
        SequentialFailed --> [*]
        ThrowAggregate --> [*]
    }
```

---

## 5. Component Dependency Graph

```mermaid
graph TD
    classDef core fill:#e1f5fe,stroke:#01579b,stroke-width:2px;
    classDef infra fill:#f3e5f5,stroke:#4a148c,stroke-width:2px;
    classDef tool fill:#fff3e0,stroke:#e65100,stroke-width:2px;
    classDef sample fill:#e8f5e9,stroke:#1b5e20,stroke-width:2px;

    Core["EricksonLopez.Mediator<br/>(netstandard2.0, net8.0, net9.0, net10.0)"]:::core
    Generator["EricksonLopez.Mediator.Generator<br/>(Roslyn Source Generator)"]:::tool

    AspNetCore["EricksonLopez.Mediator.AspNetCore"]:::infra
    Caching["EricksonLopez.Mediator.Caching"]:::infra
    FluentVal["EricksonLopez.Mediator.FluentValidation"]:::infra
    OTel["EricksonLopez.Mediator.OpenTelemetry"]:::infra
    Polly["EricksonLopez.Mediator.Polly (Deprecated)"]:::infra
    RateLim["EricksonLopez.Mediator.RateLimiting"]:::infra
    Result["EricksonLopez.Mediator.Result"]:::infra
    Testing["EricksonLopez.Mediator.Testing"]:::infra

    Showcase["EricksonLopez.Mediator.Samples<br/>(Official Living Reference Showcase)"]:::sample

    AspNetCore --> Core
    Caching --> Core
    Caching --> Result
    FluentVal --> Core
    FluentVal --> Result
    OTel --> Core
    Polly --> Core
    RateLim --> Core
    Result --> Core
    Testing --> Core

    Showcase --> Core
    Showcase --> AspNetCore
    Showcase --> Caching
    Showcase --> FluentVal
    Showcase --> OTel
    Showcase --> Polly
    Showcase --> RateLim
    Showcase --> Result
    Showcase --> Testing
    Showcase -.->|Analyzer Reference| Generator
```

---

## 6. Pipeline Architecture & Zero-Allocation Struct Continuations

```mermaid
classDiagram
    class IPipelineBehavior~TRequest, TResponse~ {
        <<interface>>
        +Handle~TNext~(request: TRequest, next: TNext, cancellationToken: CancellationToken) ValueTask~TResponse~
    }

    class INext~TResponse~ {
        <<interface>>
        +InvokeAsync() ValueTask~TResponse~
    }

    class INotificationBehavior~TNotification~ {
        <<interface>>
        +Handle~TNext~(notification: TNotification, next: TNext, cancellationToken: CancellationToken) ValueTask
    }

    class INext {
        <<interface>>
        +InvokeAsync() ValueTask
    }

    class StructNextWrapper1 {
        <<struct>>
        -behavior2: IPipelineBehavior
        -innerNext: StructNextWrapper2
        +InvokeAsync() ValueTask~TResponse~
    }

    class StructNextWrapper2 {
        <<struct>>
        -handler: ICommandHandler
        +InvokeAsync() ValueTask~TResponse~
    }

    class DelegateNext~TResponse~ {
        <<struct>>
        -continuation: Func~ValueTask~TResponse~~
        +InvokeAsync() ValueTask~TResponse~
    }

    class DelegateNext {
        <<struct>>
        -continuation: Func~ValueTask~
        +InvokeAsync() ValueTask
    }

    IPipelineBehavior ..> INext : Uses in Handle
    INotificationBehavior ..> INext : Uses in Handle
    StructNextWrapper1 ..|> INext : Implements
    StructNextWrapper2 ..|> INext : Implements
    DelegateNext ..|> INext : Implements (Testing)
    DelegateNext ..|> INext : Implements (Testing)
```

---

## 7. Processing Models (Concurrency, Batching & Streaming)

```mermaid
flowchart TD
    subgraph StreamPattern["1. Asynchronous Streaming (IStreamRequest<T>)"]
        SReq[IStreamRequest<T>] --> SHandler[IStreamRequestHandler]
        SHandler --> GeneratorYield["yield return item (IAsyncEnumerable<T>)"]
        GeneratorYield --> ConsumerEnumerator["await foreach (var item in stream)"]
        ConsumerEnumerator --> ProcessChunk[Immediate Chunk Processing]
    end

    subgraph ParallelPub["2. Concurrent Publishing ([PublishStrategy(Parallel)])"]
        PNotif[INotification] --> TaskWhenAllSplit["Task.WhenAll"]
        TaskWhenAllSplit --> HandlerA["NotificationHandler A (Search Indexing)"]
        TaskWhenAllSplit --> HandlerB["NotificationHandler B (Cache Invalidation)"]
        TaskWhenAllSplit --> HandlerC["NotificationHandler C (Dashboard Metrics)"]
        HandlerA & HandlerB & HandlerC --> TaskWhenAllJoin[Concurrent Completion of All Tasks]
    end

    subgraph RateLimitedProc["3. Controlled Throughput (RateLimitingBehavior)"]
        Request[Incoming Request] --> AcquireLease["RateLimiter.AcquireAsync(1, ct)"]
        AcquireLease --> LeaseDecision{Lease Acquired?}
        LeaseDecision -->|Yes| PipelineContinue[Execute Pipeline and Handler]
        LeaseDecision -->|No| ThrowRateLimit["Throw RateLimitExceededException(RetryAfter)"]
    end
```

---

## 8. Error Handling and Resilience Architecture

```mermaid
flowchart TD
    InputReq[Request Dispatched] --> ValidateStage{Validation Failure?}

    ValidateStage -->|Declarative: Violation| EmittedCheck{Roslyn Generator Check}
    EmittedCheck -->|Constraint Violation| ThrowValEx["Throw MediatorValidationException (IReadOnlyList<string> Errors)"]

    ValidateStage -->|FluentValidation: Invalid| ResultFactoryCheck{IResultFactory<TResponse> Registered?}
    ResultFactoryCheck -->|Yes| ReturnFailureResult["Return Result<T>.Failure(Error) Without Throwing Exception"]
    ResultFactoryCheck -->|No| ThrowFVEx["Throw ValidationException"]

    ValidateStage -->|Valid| RateLimitStage{Rate Limit Exceeded?}
    RateLimitStage -->|Yes| ThrowRateEx["Throw RateLimitExceededException (RetryAfter)"]
    RateLimitStage -->|No| ExecutionStage[Handler Execution]

    ExecutionStage --> HandlerError{Exception Thrown?}
    HandlerError -->|Yes: In Aggregate Notification| CollectAggEx["Collect in NotificationHandlerAggregateException"]
    HandlerError -->|Yes: In Standard Command/Query| UnhandledBubble["Unhandled Exception Propagation / Activity.SetStatus(Error)"]
    HandlerError -->|No| SuccessResponse["Successful Return of TResponse"]

    CollectAggEx --> ThrowAggEx["Throw NotificationHandlerAggregateException (HandlerExceptions)"]
```
