# Public API Surface Declaration

## 1. Core Namespace: `EricksonLopez.Mediator`
- `ISender`
- `IPublisher`
- `IMediator`
- `ICommand<out TResponse>`
- `IQuery<out TResponse>`
- `IStreamRequest<out TResponse>`
- `INotification`
- `ICommandHandler<in TCommand, TResponse>`
- `IQueryHandler<in TQuery, TResponse>`
- `IStreamRequestHandler<in TRequest, out TResponse>`
- `INotificationHandler<in TNotification>`
- `IPipelineBehavior<TRequest, TResponse>`
- `INotificationBehavior<TNotification>`
- `INext<TResponse>`
- `INext`
- `[UseBehaviorAttribute]`
- `[UseGlobalBehaviorAttribute]`
- `[ServiceLifetimeAttribute]`
- `[PublishStrategyAttribute]`
- `[DiscoverHandlersAttribute]`

## 2. Result Namespace: `EricksonLopez.Mediator.Result`
- `IResultFactory<TResponse>`
