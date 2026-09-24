// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.Mediator.Generator.Tests;

public class GeneratorZeroMutantCoverageTests
{
    private static Compilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICommand<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.Unsafe).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IAsyncEnumerable<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ValueTask<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Collections.ObjectModel.Collection<>).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll"))
        };

        return CSharpCompilation.Create("TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static (string DispatcherCode, string DiCode, ImmutableArray<Diagnostic> Diagnostics) RunGenerator(string source)
    {
        var compilation = CreateCompilation(source);
        var generator = new MediatorSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var runResult = driver.GetRunResult();
        var dispTree = runResult.GeneratedTrees.FirstOrDefault(t => t.FilePath.EndsWith("GeneratedMediator.g.cs"));
        var diTree = runResult.GeneratedTrees.FirstOrDefault(t => t.FilePath.EndsWith("GeneratedMediatorExtensions.g.cs"));

        return (dispTree?.ToString() ?? string.Empty, diTree?.ToString() ?? string.Empty, diagnostics);
    }

    [Fact]
    public void Dispatcher_AllMethodSignaturesAndGuards_EmittedExhaustively()
    {
        var source = @"
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace App
{
    public record TestCmd() : ICommand<int>;
    public class TestCmdHandler : ICommandHandler<TestCmd, int> { public ValueTask<int> Handle(TestCmd c, CancellationToken ct) => new(1); }

    public record TestQry() : IQuery<string>;
    public class TestQryHandler : IQueryHandler<TestQry, string> { public ValueTask<string> Handle(TestQry q, CancellationToken ct) => new(""ok""); }

    public record TestEvt() : INotification;
    public class TestEvtHandler : INotificationHandler<TestEvt> { public ValueTask Handle(TestEvt e, CancellationToken ct) => default; }

    public record TestStream() : IStreamRequest<int>;
    public class TestStreamHandler : IStreamRequestHandler<TestStream, int>
    {
        public async IAsyncEnumerable<int> Handle(TestStream s, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct) { yield return 1; }
    }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var normalizedDisp = disp.Replace("\r\n", "\n");

        // Verify Command methods
        normalizedDisp.Should().Contain("public ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)");
        normalizedDisp.Should().Contain("public ValueTask<TResponse> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)\n            where TCommand : ICommand<TResponse>");
        normalizedDisp.Should().Contain("return Send<TResponse>((ICommand<TResponse>)command, cancellationToken);");

        // Verify Query methods
        normalizedDisp.Should().Contain("public ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)");
        normalizedDisp.Should().Contain("public ValueTask<TResponse> SendQuery<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)\n            where TQuery : IQuery<TResponse>");
        normalizedDisp.Should().Contain("return Send<TResponse>((IQuery<TResponse>)query, cancellationToken);");

        // Verify Notification method
        normalizedDisp.Should().Contain("public async ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)\n            where TNotification : INotification");

        // Verify Stream method
        normalizedDisp.Should().Contain("public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)");

        // Verify default error messages
        normalizedDisp.Should().Contain("throw new InvalidOperationException($\"ELM001: No command handler found for {command.GetType()}\");");
        normalizedDisp.Should().Contain("throw new InvalidOperationException($\"ELM001: No query handler found for {query.GetType()}\");");

        // Verify exactly 6 cancellation checks and null checks across the 6 methods
        int cancelChecks = disp.Split("cancellationToken.ThrowIfCancellationRequested();").Length - 1;
        cancelChecks.Should().Be(6);

        normalizedDisp.Should().Contain("ArgumentNullException.ThrowIfNull(command);");
        normalizedDisp.Should().Contain("ArgumentNullException.ThrowIfNull(query);");
        normalizedDisp.Should().Contain("ArgumentNullException.ThrowIfNull(notification);");
        normalizedDisp.Should().Contain("ArgumentNullException.ThrowIfNull(request);");
    }

    [Fact]
    public void Dispatcher_PipelineStructs_ConstructorAndInvokeAsync_EmittedExhaustively()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace PipelineTest
{
    [UseBehavior(typeof(CmdBehavior))]
    public record SampleCmd() : ICommand<int>;
    public class SampleCmdH : ICommandHandler<SampleCmd, int> { public ValueTask<int> Handle(SampleCmd c, CancellationToken ct) => new(1); }
    public class CmdBehavior : IPipelineBehavior<SampleCmd, int>
    {
        public ValueTask<int> Handle<TNext>(SampleCmd r, TNext n, CancellationToken ct) where TNext : struct, INext<int> => n.InvokeAsync();
    }

    [UseBehavior(typeof(NotifBehavior))]
    public record SampleEvt() : INotification;
    public class SampleEvtH : INotificationHandler<SampleEvt> { public ValueTask Handle(SampleEvt e, CancellationToken ct) => default; }
    public class NotifBehavior : INotificationBehavior<SampleEvt>
    {
        public ValueTask Handle<TNext>(SampleEvt n, TNext next, CancellationToken ct) where TNext : struct, INext => next.InvokeAsync();
    }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var normalizedDisp = disp.Replace("\r\n", "\n");

        // Command Behavior struct
        var expectedCmdStruct =
@"    internal readonly struct PipelineTest_SampleCmdBehavior0Next : INext<int>
    {
        private readonly global::PipelineTest.CmdBehavior _behavior;
        private readonly PipelineTest_SampleCmdHandlerNext _next;
        private readonly global::PipelineTest.SampleCmd _request;
        private readonly CancellationToken _ct;

        public PipelineTest_SampleCmdBehavior0Next(global::PipelineTest.CmdBehavior behavior, PipelineTest_SampleCmdHandlerNext next, global::PipelineTest.SampleCmd request, CancellationToken ct)
        {
            _behavior = behavior;
            _next = next;
            _request = request;
            _ct = ct;
        }

        public ValueTask<int> InvokeAsync() => _behavior.Handle(_request, _next, _ct);
    }";
        normalizedDisp.Should().Contain(expectedCmdStruct);

        // Notification Behavior struct
        var expectedNotifStruct =
@"    internal readonly struct PipelineTest_SampleEvtBehavior0Next : INext
    {
        private readonly global::PipelineTest.NotifBehavior _behavior;
        private readonly PipelineTest_SampleEvtNotificationNext _next;
        private readonly global::PipelineTest.SampleEvt _request;
        private readonly CancellationToken _ct;

        public PipelineTest_SampleEvtBehavior0Next(global::PipelineTest.NotifBehavior behavior, PipelineTest_SampleEvtNotificationNext next, global::PipelineTest.SampleEvt request, CancellationToken ct)
        {
            _behavior = behavior;
            _next = next;
            _request = request;
            _ct = ct;
        }

        public ValueTask InvokeAsync() => _behavior.Handle(_request, _next, _ct);
    }";
        normalizedDisp.Should().Contain(expectedNotifStruct);
    }

    [Fact]
    public void Dispatcher_ParallelNotification_EmitsFullTryCatchAndWhenAll()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace ParTest
{
    [PublishStrategy(PublishStrategy.Parallel)]
    public record ParEvt() : INotification;
    public class ParH1 : INotificationHandler<ParEvt> { public ValueTask Handle(ParEvt e, CancellationToken ct) => default; }
    public class ParH2 : INotificationHandler<ParEvt> { public ValueTask Handle(ParEvt e, CancellationToken ct) => default; }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var normalizedDisp = disp.Replace("\r\n", "\n");

        var expectedTryCatch =
@"            var allTasks = Task.WhenAll(tasks);
            try
            {
                await allTasks.ConfigureAwait(false);
            }
            catch
            {
                if (allTasks.Exception is { InnerExceptions.Count: > 0 } agg)
                {
                    throw new global::EricksonLopez.Mediator.NotificationHandlerAggregateException(agg.InnerExceptions);
                }
                throw;
            }";
        normalizedDisp.Should().Contain(expectedTryCatch);
    }

    [Fact]
    public void Dispatcher_Validation_ExhaustiveRulesAndCustomMessages()
    {
        var source = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace ValTest
{
    public record CustomMsgCmd(
        [property: ValidateNotNull(""Custom Null Msg"")] string? NullProp,
        [property: ValidateNotEmpty(""Custom Empty Msg"")] string? EmptyProp,
        [property: ValidateRange(5, 100, ""Custom Range Msg"")] int RangeProp,
        [property: ValidateLength(1, 10, ""Custom Length Msg"")] string? LenProp,
        [property: ValidateRegex(""^[0-9]+$"", ""Custom Regex Msg"")] string? RegexProp
    ) : ICommand<int>;

    public class CustomMsgCmdHandler : ICommandHandler<CustomMsgCmd, int>
    {
        public ValueTask<int> Handle(CustomMsgCmd c, CancellationToken ct) => new(1);
    }

    public class CustomSeq : System.Collections.IEnumerable
    {
        public System.Collections.IEnumerator GetEnumerator() => throw new NotImplementedException();
    }

    public record LengthTypesCmd(
        [property: ValidateLength(1, 10)] string Str,
        [property: ValidateLength(1, 10)] string? NullableStr,
        [property: ValidateLength(1, 10)] int[] ArrayProp,
        [property: ValidateLength(1, 10)] List<int> ListProp,
        [property: ValidateLength(1, 10)] Collection<int> ColProp,
        [property: ValidateLength(1, 10)] HashSet<int> SetProp,
        [property: ValidateLength(1, 10)] Queue<int> QueueProp,
        [property: ValidateLength(1, 10)] Stack<int> StackProp,
        [property: ValidateLength(1, 10)] Dictionary<string, int> DictProp,
        [property: ValidateLength(1, 10)] CustomSeq SeqProp
    ) : ICommand<int>;

    public class LengthTypesCmdHandler : ICommandHandler<LengthTypesCmd, int>
    {
        public ValueTask<int> Handle(LengthTypesCmd c, CancellationToken ct) => new(1);
    }

    public record RegexEscapingCmd(
        [property: ValidateRegex(""path\\\\dir\\\""file\\r\\n\\t"")] string SpecialRegex
    ) : ICommand<int>;

    public class RegexEscapingCmdHandler : ICommandHandler<RegexEscapingCmd, int>
    {
        public ValueTask<int> Handle(RegexEscapingCmd c, CancellationToken ct) => new(1);
    }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        // Verify Custom Messages
        disp.Should().Contain("throw new global::EricksonLopez.Mediator.MediatorValidationException(\"Custom Null Msg\");");
        disp.Should().Contain("throw new global::EricksonLopez.Mediator.MediatorValidationException(\"Custom Empty Msg\");");
        disp.Should().Contain("throw new global::EricksonLopez.Mediator.MediatorValidationException(\"Custom Range Msg\");");
        disp.Should().Contain("throw new global::EricksonLopez.Mediator.MediatorValidationException(\"Custom Length Msg\");");
        disp.Should().Contain("throw new global::EricksonLopez.Mediator.MediatorValidationException(\"Custom Regex Msg\");");

        // Verify Length accessors
        disp.Should().Contain("req.Str.Length < 1 || req.Str.Length > 10");
        disp.Should().Contain("req.NullableStr.Length < 1 || req.NullableStr.Length > 10");
        disp.Should().Contain("req.ArrayProp.Length < 1 || req.ArrayProp.Length > 10");
        disp.Should().Contain("req.ListProp.Count < 1 || req.ListProp.Count > 10");
        disp.Should().Contain("req.ColProp.Count < 1 || req.ColProp.Count > 10");
        disp.Should().Contain("req.SetProp.Count < 1 || req.SetProp.Count > 10");
        disp.Should().Contain("req.QueueProp.Count < 1 || req.QueueProp.Count > 10");
        disp.Should().Contain("req.StackProp.Count < 1 || req.StackProp.Count > 10");
        disp.Should().Contain("req.DictProp.Count < 1 || req.DictProp.Count > 10");
        disp.Should().Contain("global::System.Linq.Enumerable.Count(req.SeqProp) < 1 || global::System.Linq.Enumerable.Count(req.SeqProp) > 10");

        // Verify Regex escaping for \, ", \r, \n, \t
        disp.Should().Contain(@"path\\\\dir\\\""file\\r\\n\\t");
    }

    [Fact]
    public void ModelBuilder_PrivateAndProtectedHandlers_AreSkipped()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace AccessTest
{
    public record PublicCmd() : ICommand<int>;

    public class Container
    {
        private class PrivateHandler : ICommandHandler<PublicCmd, int>
        {
            public ValueTask<int> Handle(PublicCmd c, CancellationToken ct) => new(1);
        }

        protected class ProtectedHandler : ICommandHandler<PublicCmd, int>
        {
            public ValueTask<int> Handle(PublicCmd c, CancellationToken ct) => new(2);
        }

        private protected class PrivateProtectedHandler : ICommandHandler<PublicCmd, int>
        {
            public ValueTask<int> Handle(PublicCmd c, CancellationToken ct) => new(3);
        }
    }
}";
        var (disp, di, diags) = RunGenerator(source);

        di.Should().NotContain("PrivateHandler");
        di.Should().NotContain("ProtectedHandler");
        di.Should().NotContain("PrivateProtectedHandler");
    }

    [Fact]
    public void ModelBuilder_IRequestHandler_QueryAndCommandAndGenerics_HandledCorrectly()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace ReqHandlerTest
{
    public record ReqQuery(int Id) : IQuery<string>;
    public class ReqQueryH : IRequestHandler<ReqQuery, string>
    {
        public ValueTask<string> Handle(ReqQuery q, CancellationToken ct) => new(""result"");
    }

    public record ReqCmd(int Id) : IRequest<int>;
    public class ReqCmdH : IRequestHandler<ReqCmd, int>
    {
        public ValueTask<int> Handle(ReqCmd c, CancellationToken ct) => new(c.Id);
    }

    public class OpenGenericReqH<TReq, TRes> : IRequestHandler<TReq, TRes>
        where TReq : IRequest<TRes>
    {
        public ValueTask<TRes> Handle(TReq req, CancellationToken ct) => default;
    }

    public record BadSigReq() : IRequest<int>;
    public class BadSigReqH : IRequestHandler<BadSigReq, int>
    {
        public int Handle(BadSigReq req) => 1;
    }
}";
        var (disp, di, diags) = RunGenerator(source);

        // ReqQuery should be recognized as a query and appear in Query dispatch
        disp.Should().Contain("case global::ReqHandlerTest.ReqQuery req:");
        di.Should().Contain("services.TryAddTransient<global::ReqHandlerTest.ReqQueryH>();");

        // ReqCmd should be recognized as command
        disp.Should().Contain("case global::ReqHandlerTest.ReqCmd req:");
        di.Should().Contain("services.TryAddTransient<global::ReqHandlerTest.ReqCmdH>();");

        // Open generic handler should emit ELM005
        diags.Should().Contain(d => d.Id == "ELM005" && d.Severity == DiagnosticSeverity.Warning && d.GetMessage().Contains("OpenGenericReqH"));

        // Bad signature handler should emit ELM004
        diags.Should().Contain(d => d.Id == "ELM004" && d.Severity == DiagnosticSeverity.Error && d.GetMessage().Contains("BadSigReqH"));
    }

    [Fact]
    public void ModelBuilder_SingleTypeParamBehavior_ConstraintFiltering_Works()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace SingleParamTest
{
    public interface IAuditable { }
    public abstract record BaseCommand() : ICommand<int>;
    public record DerivedCommand() : BaseCommand;
    public class DerivedCommandH : ICommandHandler<DerivedCommand, int> { public ValueTask<int> Handle(DerivedCommand c, CancellationToken ct) => new(1); }

    public record ConcreteCommand() : ICommand<int>;
    public class ConcreteCommandH : ICommandHandler<ConcreteCommand, int> { public ValueTask<int> Handle(ConcreteCommand c, CancellationToken ct) => new(1); }

    public record AuditedCommand() : ICommand<int>, IAuditable;
    public class AuditedCommandH : ICommandHandler<AuditedCommand, int> { public ValueTask<int> Handle(AuditedCommand c, CancellationToken ct) => new(1); }

    public record PlainCommand() : ICommand<int>;
    public class PlainCommandH : ICommandHandler<PlainCommand, int> { public ValueTask<int> Handle(PlainCommand c, CancellationToken ct) => new(1); }

    [UseBehavior(typeof(AuditBehavior<>))]
    public record DirectAnnotatedNonAuditedCommand() : ICommand<int>;
    public class DirectAnnotatedNonAuditedCommandH : ICommandHandler<DirectAnnotatedNonAuditedCommand, int> { public ValueTask<int> Handle(DirectAnnotatedNonAuditedCommand c, CancellationToken ct) => new(1); }

    [UseBehavior(typeof(BaseCommandBehavior<>))]
    public record AnnotatedDerivedCommand() : BaseCommand;
    public class AnnotatedDerivedCommandH : ICommandHandler<AnnotatedDerivedCommand, int> { public ValueTask<int> Handle(AnnotatedDerivedCommand c, CancellationToken ct) => new(1); }

    [UseBehavior(typeof(ExactCommandBehavior<>))]
    public record AnnotatedConcreteCommand() : ConcreteCommand;
    public class AnnotatedConcreteCommandH : ICommandHandler<AnnotatedConcreteCommand, int> { public ValueTask<int> Handle(AnnotatedConcreteCommand c, CancellationToken ct) => new(1); }

    public class AuditBehavior<TReq> : IPipelineBehavior<TReq, int> where TReq : IAuditable
    {
        public ValueTask<int> Handle<TNext>(TReq request, TNext next, CancellationToken cancellationToken) where TNext : struct, INext<int> => next.InvokeAsync();
    }

    public class BaseCommandBehavior<TReq> : IPipelineBehavior<TReq, int> where TReq : BaseCommand
    {
        public ValueTask<int> Handle<TNext>(TReq request, TNext next, CancellationToken cancellationToken) where TNext : struct, INext<int> => next.InvokeAsync();
    }

    public class ExactCommandBehavior<TReq> : IPipelineBehavior<TReq, int> where TReq : ConcreteCommand
    {
        public ValueTask<int> Handle<TNext>(TReq request, TNext next, CancellationToken cancellationToken) where TNext : struct, INext<int> => next.InvokeAsync();
    }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        // DirectAnnotatedNonAuditedCommand does not implement IAuditable, so AuditBehavior should NOT be wrapped
        disp.Should().NotContain("DirectAnnotatedNonAuditedCommandBehavior0Next");

        // BaseCommand constraint matches BaseType
        disp.Should().Contain("AnnotatedDerivedCommandBehavior0Next");

        // ExactCommand constraint matches exact type or BaseType
        disp.Should().Contain("AnnotatedConcreteCommandBehavior0Next");
    }

    [Fact]
    public void ModelBuilder_GlobalBehavior_OnNotification_IsIncludedInPipeline()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

[assembly: UseGlobalBehavior(typeof(NotifApp.GlobalNotifBehavior), 1)]

namespace NotifApp
{
    public record MyGlobalEvent() : INotification;
    public class MyGlobalEventH : INotificationHandler<MyGlobalEvent> { public ValueTask Handle(MyGlobalEvent e, CancellationToken ct) => default; }

    public class GlobalNotifBehavior : INotificationBehavior<MyGlobalEvent>
    {
        public ValueTask Handle<TNext>(MyGlobalEvent n, TNext next, CancellationToken ct) where TNext : struct, INext => next.InvokeAsync();
    }
}";
        var (disp, di, diags) = RunGenerator(source);
        diags.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        disp.Should().Contain("NotifApp_MyGlobalEventBehavior0Next");
        disp.Should().Contain("_serviceProvider.GetRequiredService<global::NotifApp.GlobalNotifBehavior>()");
    }

    [Fact]
    public void ModelBuilder_InvalidSignatures_EmitExactELM004Descriptors()
    {
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace InvalidSigTest
{
    public record BrokenCmd() : ICommand<int>;
    public class BrokenCmdH : ICommandHandler<BrokenCmd, int>
    {
        public void Handle(BrokenCmd c) { }
    }

    public record BrokenNotif() : INotification;
    public class BrokenNotifH : INotificationHandler<BrokenNotif>
    {
        public void Handle(BrokenNotif n) { }
    }

    public class BrokenBehavior : IPipelineBehavior<BrokenCmd, int>
    {
        public ValueTask<int> Handle(BrokenCmd c) => new(1);
    }
}";
        var (disp, di, diags) = RunGenerator(source);

        var cmdDiag = diags.FirstOrDefault(d => d.Id == "ELM004" && d.GetMessage().Contains("BrokenCmdH"));
        cmdDiag.Should().NotBeNull();
        cmdDiag!.Descriptor.Title.ToString().Should().Be("Invalid Handler Signature");

        var notifDiag = diags.FirstOrDefault(d => d.Id == "ELM004" && d.GetMessage().Contains("BrokenNotifH"));
        notifDiag.Should().NotBeNull();
        notifDiag!.Descriptor.Title.ToString().Should().Be("Invalid Handler Signature");

        var behaviorDiag = diags.FirstOrDefault(d => d.Id == "ELM004" && d.GetMessage().Contains("BrokenBehavior"));
        behaviorDiag.Should().NotBeNull();
        behaviorDiag!.Descriptor.Title.ToString().Should().Be("Invalid Behavior Signature");
    }
}
