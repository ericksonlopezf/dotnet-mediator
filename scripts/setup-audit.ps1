$ErrorActionPreference = "Stop"
$baseDir = "MEGA-AUDIT"

$directories = @(
    "evidence/inventory",
    "evidence/architecture",
    "evidence/api",
    "evidence/correctness",
    "evidence/di",
    "evidence/pipeline",
    "evidence/source-generator",
    "evidence/concurrency",
    "evidence/performance",
    "evidence/security",
    "evidence/reliability",
    "evidence/aot",
    "evidence/observability",
    "evidence/compatibility",
    "evidence/ecosystem",
    "tests/adversarial",
    "tests/concurrency",
    "tests/fuzzing",
    "tests/property-based",
    "tests/regression",
    "tests/integration",
    "benchmarks",
    "mutation",
    "aot",
    "fuzzing",
    "reports"
)

foreach ($dir in $directories) {
    $fullPath = Join-Path $baseDir $dir
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
    }
}

Write-Host "All MEGA-AUDIT directories verified."
