// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.Mediator.Generator.Tests;

/// <summary>
/// Represents GeneratorAdvancedTests.
/// </summary>
public partial class GeneratorAdvancedTests
{

    /// <summary>
    /// Executes EmptyCompilation_DoesNotGenerateFiles.
    /// </summary>
    [Fact]
    public void EmptyCompilation_DoesNotGenerateFiles()
    {
        string source = @"
namespace TestApp
{
    /// <summary>
    /// Represents NormalClass.
    /// </summary>
    public class NormalClass { }
}
";
        var compilation = CreateCompilation(source);
        var generator = new MediatorSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        // Only the original syntax tree should be present
        Assert.Single(generatedSyntaxTrees);
    }
    /// <summary>
    /// Executes Dispatcher_GeneratesPipeline_WhenBehaviorsArePresent.
    /// </summary>
    [Fact]
    public void Dispatcher_GeneratesPipeline_WhenBehaviorsArePresent()
    {
        string source = @"

[assembly: EricksonLopez.Mediator.UseGlobalBehavior(typeof(TestApp.LoggingBehavior))]
[assembly: EricksonLopez.Mediator.UseGlobalBehavior(typeof(TestApp.ValidationBehavior), 2)]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(string))]

namespace TestApp
{
    /// <summary>
    /// Represents MyCommand.
    /// </summary>
    [UseBehavior(typeof(SpecificBehavior))]
    [UseBehavior(typeof(ClosedGenericBehavior<MyCommand>))]
    [System.Obsolete]
    public class MyCommand : ICommand<int> { }
    
    /// <summary>
    /// Represents MyCommandHandler.
    /// </summary>
    public class MyCommandHandler : ICommandHandler<MyCommand, int>
    {
        /// <summary>
        /// Executes Handle.
        /// </summary>
        public ValueTask<int> Handle(MyCommand command, CancellationToken ct) => new(42);
    }

    /// <summary>
    /// Represents LoggingBehavior.
    /// </summary>
    public class LoggingBehavior : IPipelineBehavior<MyCommand, int>
    {
        /// <summary>
        /// Executes Handle.
        /// </summary>
        public ValueTask<int> Handle<TNext>(MyCommand req, TNext next, CancellationToken ct) where TNext : struct, INext<int> => next.InvokeAsync();
    }
    
    /// <summary>
    /// Represents ValidationBehavior.
    /// </summary>
    public class ValidationBehavior : IPipelineBehavior<MyCommand, int>
    {
        /// <summary>
        /// Executes Handle.
        /// </summary>
        public ValueTask<int> Handle<TNext>(MyCommand req, TNext next, CancellationToken ct) where TNext : struct, INext<int> => next.InvokeAsync();
    }

    /// <summary>
    /// Represents SpecificBehavior.
    /// </summary>
    public class SpecificBehavior : IPipelineBehavior<MyCommand, int>
    {
        /// <summary>
        /// Executes Handle.
        /// </summary>
        public ValueTask<int> Handle<TNext>(MyCommand req, TNext next, CancellationToken ct) where TNext : struct, INext<int> => next.InvokeAsync();
    }

    /// <summary>
    /// Represents ClosedGenericBehavior.
    /// </summary>
    public class ClosedGenericBehavior<TRequest> : IPipelineBehavior<TRequest, int> where TRequest : ICommand<int>
    {
        /// <summary>
        /// Executes Handle.
        /// </summary>
        public ValueTask<int> Handle<TNext>(TRequest req, TNext next, CancellationToken ct) where TNext : struct, INext<int> => next.InvokeAsync();
    }
}
";
        var compilation = CreateCompilation(source);

        var compDiagnostics = compilation.GetDiagnostics();
        compDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var generator = new MediatorSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        errors.Should().BeEmpty();

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        var dispatcherCode = generatedSyntaxTrees.First(t => t.FilePath.Contains("GeneratedMediator.g.cs")).ToString();

        // Should generate structs for next
        dispatcherCode.Should().Contain("internal readonly struct TestApp_MyCommandHandlerNext");
        dispatcherCode.Should().Contain("internal readonly struct TestApp_MyCommandBehavior0Next");
        dispatcherCode.Should().Contain("internal readonly struct TestApp_MyCommandBehavior1Next");
        // Should chain them in the switch case
        Assert.Contains("var handlerNext = new TestApp_MyCommandHandlerNext(_serviceProvider, req, cancellationToken);", dispatcherCode);
        dispatcherCode.Should().Contain("var b3 = _serviceProvider.GetRequiredService<global::TestApp.ValidationBehavior>();");
        dispatcherCode.Should().Contain("var b2 = _serviceProvider.GetRequiredService<global::TestApp.ClosedGenericBehavior<global::TestApp.MyCommand>>();");
        dispatcherCode.Should().Contain("var b1 = _serviceProvider.GetRequiredService<global::TestApp.SpecificBehavior>();");
        dispatcherCode.Should().Contain("var b0 = _serviceProvider.GetRequiredService<global::TestApp.LoggingBehavior>();");
    }

    /// <summary>
    /// Executes Dispatcher_GeneratesParallelNotification_WhenStrategyIsParallel.
    /// </summary>
    [Fact]
    public void Dispatcher_GeneratesParallelNotification_WhenStrategyIsParallel()
    {
        string source = @"
namespace TestApp
{
    [PublishStrategy(PublishStrategy.Parallel)]
    public class MyEvent : INotification { }

    public class Handler1 : INotificationHandler<MyEvent>
    {
        public ValueTask Handle(MyEvent notification, CancellationToken ct) => default;
    }

    public class Handler2 : INotificationHandler<MyEvent>
    {
        public ValueTask Handle(MyEvent notification, CancellationToken ct) => default;
    }
}";

        var compilation = CreateCompilation(source);
        var driver = CSharpGeneratorDriver.Create(new MediatorSourceGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        var dispatcherCode = generatedSyntaxTrees.First(t => t.FilePath.Contains("GeneratedMediator.g.cs")).ToString();

        dispatcherCode.Should().Contain("var tasks = new Task[2];");
        Assert.Contains("tasks[0] = _sp.GetRequiredService<global::TestApp.Handler1>().Handle(_n, _ct).AsTask();", dispatcherCode);
        Assert.Contains("tasks[1] = _sp.GetRequiredService<global::TestApp.Handler2>().Handle(_n, _ct).AsTask();", dispatcherCode);
        dispatcherCode.Should().Contain("await allTasks.ConfigureAwait(false);");
    }

    /// <summary>
    /// Executes Dispatcher_GeneratesSequentialNotification_WhenStrategyIsSequential.
    /// </summary>
    [Fact]
    public void Dispatcher_GeneratesSequentialNotification_WhenStrategyIsSequential()
    {
        string source = @"
namespace TestApp
{
    [PublishStrategy(PublishStrategy.Sequential)]
    public class MyEvent : INotification { }

    public class Handler1 : INotificationHandler<MyEvent>
    {
        public ValueTask Handle(MyEvent notification, CancellationToken ct) => default;
    }

    public class Handler2 : INotificationHandler<MyEvent>
    {
        public ValueTask Handle(MyEvent notification, CancellationToken ct) => default;
    }
}";

        var compilation = CreateCompilation(source);
        var driver = CSharpGeneratorDriver.Create(new MediatorSourceGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        var dispatcherCode = generatedSyntaxTrees.First(t => t.FilePath.Contains("GeneratedMediator.g.cs")).ToString();

        dispatcherCode.Should().Contain("await _sp.GetRequiredService<global::TestApp.Handler1>().Handle(_n, _ct).ConfigureAwait(false);");
        dispatcherCode.Should().Contain("await _sp.GetRequiredService<global::TestApp.Handler2>().Handle(_n, _ct).ConfigureAwait(false);");
    }

    /// <summary>
    /// Executes DependencyInjection_GeneratesCorrectRegistrations.
    /// </summary>
    [Fact]
    public void DependencyInjection_GeneratesCorrectRegistrations()
    {
        string source = @"
namespace TestApp
{
    public class MyCommand1 : ICommand<int> { }
    public class MyCommand2 : ICommand<int> { }
    public class MyCommand3 : ICommand<int> { }

    [ServiceLifetime(HandlerLifetime.Transient)]
    public class TransientHandler : ICommandHandler<MyCommand1, int> { public ValueTask<int> Handle(MyCommand1 command, CancellationToken ct) => default; }

    [ServiceLifetime(HandlerLifetime.Scoped)]
    public class ScopedHandler : ICommandHandler<MyCommand2, int> { public ValueTask<int> Handle(MyCommand2 command, CancellationToken ct) => default; }

    [ServiceLifetime(HandlerLifetime.Singleton)]
    public class SingletonHandler : ICommandHandler<MyCommand3, int> { public ValueTask<int> Handle(MyCommand3 command, CancellationToken ct) => default; }

    public class SomeQuery : IQuery<int> { }
    public class QueryHandler : IQueryHandler<SomeQuery, int> { public ValueTask<int> Handle(SomeQuery query, CancellationToken ct) => default; }

    public class SomeEvent : INotification { }
    public class EventHandler : INotificationHandler<SomeEvent> { public ValueTask Handle(SomeEvent notification, CancellationToken ct) => default; }
}";

        var compilation = CreateCompilation(source);
        var driver = CSharpGeneratorDriver.Create(new MediatorSourceGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        var diCode = generatedSyntaxTrees.First(t => t.FilePath.Contains("GeneratedMediatorExtensions.g.cs")).ToString();

        string expected = @"// <auto-generated/>
#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Generated;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeneratedMediatorExtensions
    {
        /// <summary>
        /// Registers mediator components into the <see cref=""IServiceCollection""/> with Scoped lifetime by default (ADR-037).
        /// </summary>
        /// <param name=""services"">The service collection to register into.</param>
        /// <param name=""lifetime"">The service lifetime for <see cref=""IMediator""/>, <see cref=""ISender""/>, and <see cref=""IPublisher""/> (default: <see cref=""ServiceLifetime.Scoped""/>).</param>
        public static IServiceCollection AddEricksonLopezMediator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(GeneratedMediator), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), lifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), lifetime));

            services.TryAddTransient<global::TestApp.TransientHandler>();
            services.TryAddScoped<global::TestApp.ScopedHandler>();
            services.TryAddSingleton<global::TestApp.SingletonHandler>();
            services.TryAddTransient<global::TestApp.QueryHandler>();
            services.TryAddTransient<global::TestApp.EventHandler>();

            return services;
        }
    }

}
";
        Assert.Equal(expected.Replace("\r\n", "\n"), diCode.Replace("\r\n", "\n"));
    }

    /// <summary>
    /// Executes Dispatcher_GeneratesCorrectStructure.
    /// </summary>
    [Fact]
    public void Dispatcher_GeneratesCorrectStructure()
    {
        string source = @"

namespace TestApp
{
    /// <summary>
    /// Represents MyCommand.
    /// </summary>
    public class MyCommand : ICommand<int> { }
    /// <summary>
    /// Represents MyCommandHandler.
    /// </summary>
    public class MyCommandHandler : ICommandHandler<MyCommand, int> { public ValueTask<int> Handle(MyCommand c, CancellationToken ct) => default; }
    
    /// <summary>
    /// Represents MySeqNotification.
    /// </summary>
    [PublishStrategy(PublishStrategy.Sequential)]
    public class MySeqNotification : INotification { }
    /// <summary>
    /// Represents MySeqNotificationHandler.
    /// </summary>
    public class MySeqNotificationHandler : INotificationHandler<MySeqNotification> { public ValueTask Handle(MySeqNotification n, CancellationToken ct) => default; }
    
    /// <summary>
    /// Represents MyParNotification.
    /// </summary>
    [PublishStrategy(PublishStrategy.Parallel)]
    public class MyParNotification : INotification { }
    /// <summary>
    /// Represents MyParNotificationHandler1.
    /// </summary>
    public class MyParNotificationHandler1 : INotificationHandler<MyParNotification> { public ValueTask Handle(MyParNotification n, CancellationToken ct) => default; }
    /// <summary>
    /// Represents MyParNotificationHandler2.
    /// </summary>
    public class MyParNotificationHandler2 : INotificationHandler<MyParNotification> { public ValueTask Handle(MyParNotification n, CancellationToken ct) => default; }
}
";
        var compilation = CreateCompilation(source);
        var generator = new MediatorSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToList();
        var dispatcherCode = generatedSyntaxTrees.First(t => t.FilePath.Contains("GeneratedMediator.g.cs")).ToString();

        dispatcherCode.Should().Contain("internal readonly struct TestApp_MyCommandHandlerNext : INext<int>");
        dispatcherCode.Should().Contain("public sealed class GeneratedMediator : IMediator");
        dispatcherCode.Should().Contain("switch (command)");
        dispatcherCode.Should().Contain("switch (notification)");
        dispatcherCode.Should().Contain("switch (request)");
        dispatcherCode.Should().Contain("internal readonly struct TestApp_MySeqNotificationNotificationNext : INext");
        dispatcherCode.Should().Contain("internal readonly struct TestApp_MyParNotificationNotificationNext : INext");
        Assert.Contains("await _sp.GetRequiredService<global::TestApp.MySeqNotificationHandler>().Handle(_n, _ct).ConfigureAwait(false);", dispatcherCode);
        Assert.Contains("tasks[0] = _sp.GetRequiredService<global::TestApp.MyParNotificationHandler1>().Handle(_n, _ct).AsTask();", dispatcherCode);
    }
}

