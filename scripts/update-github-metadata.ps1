# Copyright © Erickson Lopez. MIT License.
<#
.SYNOPSIS
    Updates GitHub repository description, website, and topics via GitHub REST API.
.DESCRIPTION
    Applies the standardized 20 topics and Option C description for ericksonlopezf/dotnet-mediator.
.PARAMETER Token
    GitHub Personal Access Token (PAT) with 'repo' scope. If not specified, reads from $env:GITHUB_TOKEN or $env:GH_TOKEN.
.EXAMPLE
    ./scripts/update-github-metadata.ps1 -Token "ghp_xxx"
#>

[CmdletBinding()]
param (
    [string]$Token = ($env:GITHUB_TOKEN, $env:GH_TOKEN | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -First 1),
    [string]$Owner = "ericksonlopezf",
    [string]$Repo = "dotnet-mediator"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($Token)) {
    Write-Host "❌ Error: GitHub Token is required. Provide -Token or set GITHUB_TOKEN environment variable." -ForegroundColor Red
    Write-Host "   Usage: ./scripts/update-github-metadata.ps1 -Token '<your-token>'" -ForegroundColor Yellow
    exit 1
}

$headers = @{
    "Authorization" = "Bearer $Token"
    "Accept"        = "application/vnd.github+json"
    "User-Agent"    = "EricksonLopez-Metadata-Updater"
}

$description = "High-throughput, zero-allocation CQRS mediator ecosystem for modern .NET (8, 9, 10). Features Roslyn compile-time dispatch, 100% Native AOT compatibility, and zero-overhead pipeline extensions for caching, telemetry, and validation."
$homepage = "https://ericksonlopez.dev/mediator"

$topics = @(
    "dotnet",
    "csharp",
    "mediator",
    "cqrs",
    "source-generator",
    "roslyn",
    "native-aot",
    "zero-allocation",
    "pipeline",
    "domain-events",
    "streaming",
    "aspnetcore",
    "minimal-apis",
    "caching",
    "fluentvalidation",
    "opentelemetry",
    "resilience",
    "rate-limiting",
    "result-pattern",
    "unit-testing"
)

Write-Host "Updating repository metadata for $Owner/$Repo..." -ForegroundColor Cyan

# 1. Update Description and Homepage
$repoBody = @{
    description = $description
    homepage    = $homepage
} | ConvertTo-Json

try {
    $repoUri = "https://api.github.com/repos/$Owner/$Repo"
    $response = Invoke-RestMethod -Uri $repoUri -Method Patch -Headers $headers -Body $repoBody -ContentType "application/json"
    Write-Host "✅ Repository description and homepage updated successfully." -ForegroundColor Green
}
catch {
    Write-Host "❌ Failed to update repository details: $_" -ForegroundColor Red
    exit 1
}

# 2. Update Topics
$topicsBody = @{
    names = $topics
} | ConvertTo-Json

try {
    $topicsUri = "https://api.github.com/repos/$Owner/$Repo/topics"
    $response = Invoke-RestMethod -Uri $topicsUri -Method Put -Headers $headers -Body $topicsBody -ContentType "application/json"
    Write-Host "✅ Repository topics updated successfully ($($topics.Count) topics applied)." -ForegroundColor Green
    Write-Host "   Topics: $($topics -join ', ')" -ForegroundColor Gray
}
catch {
    Write-Host "❌ Failed to update repository topics: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 GitHub repository metadata synchronization complete!" -ForegroundColor Green
