// Copyright © Erickson Lopez. MIT License.
using EricksonLopez.Result;

namespace EricksonLopez.Mediator.Caching.Tests;

public sealed record TestInvalidateCommand(string TenantId) : ICommand<Result<bool>>, IInvalidateCacheRequest
{
    public string CachePrefix => $"tenant:{TenantId}:";
}
