## Description
Please include a summary of the change and which issue is fixed. Please also include relevant motivation and context.

Fixes # (issue)

## Affected Packages
- [ ] `EricksonLopez.Mediator`
- [ ] `EricksonLopez.Mediator.Generator`
- [ ] `EricksonLopez.Mediator.AspNetCore`
- [ ] `EricksonLopez.Mediator.Caching`
- [ ] `EricksonLopez.Mediator.FluentValidation`
- [ ] `EricksonLopez.Mediator.OpenTelemetry`
- [ ] `EricksonLopez.Mediator.Polly`
- [ ] `EricksonLopez.Mediator.RateLimiting`
- [ ] `EricksonLopez.Mediator.Result`
- [ ] `EricksonLopez.Mediator.Testing`

## Type of change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

## Checklist:
- [ ] My code follows the style guidelines and architectural invariants of this project
- [ ] I have performed a self-review of my own code
- [ ] Public members in `src/` have complete XML documentation (CS1591 compliance)
- [ ] Repository compliance verified with zero violations (`./scripts/verify-compliance.ps1`)
- [ ] New and existing unit and integration tests pass locally (`dotnet test EricksonLopez.Mediator.slnx`)
- [ ] Native AOT smoke test compiles and executes with zero trimming warnings (`tests/EricksonLopez.Mediator.AotSmokeTest`)
- [ ] Mutation testing score satisfies quality gate thresholds (Stryker break ≥ 95%, low ≥ 98%)
- [ ] Benchmark regression gate verified with zero heap allocations on hot path (0 B allocated) and ≤ 5% latency regression
- [ ] I have made corresponding changes to the documentation in `docs/` and `README.md`
