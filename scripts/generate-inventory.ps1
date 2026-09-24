$ErrorActionPreference = "Stop"

$invDir = "MEGA-AUDIT/evidence/inventory"
if (-not (Test-Path $invDir)) {
    New-Item -ItemType Directory -Path $invDir -Force | Out-Null
}

Write-Host "Gathering Target Frameworks..."
$tfmReport = [System.Text.StringBuilder]::new()
[void]$tfmReport.AppendLine("# TARGET FRAMEWORKS & COMPILER CONFIGURATION")
[void]$tfmReport.AppendLine("Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$tfmReport.AppendLine("")

$projects = Get-ChildItem -Path "src", "tests", "benchmarks" -Recurse -Filter "*.csproj"
foreach ($proj in $projects) {
    [xml]$xml = Get-Content $proj.FullName
    $tfm = $xml.Project.PropertyGroup.TargetFramework
    $tfms = $xml.Project.PropertyGroup.TargetFrameworks
    $isAot = $xml.Project.PropertyGroup.IsAotCompatible
    $isTrim = $xml.Project.PropertyGroup.IsTrimmable
    $nullable = $xml.Project.PropertyGroup.Nullable
    $treatAsErr = $xml.Project.PropertyGroup.TreatWarningsAsErrors

    [void]$tfmReport.AppendLine("Project: $($proj.Name)")
    [void]$tfmReport.AppendLine("  Path: $($proj.FullName)")
    if ($tfm) { [void]$tfmReport.AppendLine("  TargetFramework: $tfm") }
    if ($tfms) { [void]$tfmReport.AppendLine("  TargetFrameworks: $tfms") }
    if ($nullable) { [void]$tfmReport.AppendLine("  Nullable: $nullable") }
    if ($isAot) { [void]$tfmReport.AppendLine("  IsAotCompatible: $isAot") }
    if ($isTrim) { [void]$tfmReport.AppendLine("  IsTrimmable: $isTrim") }
    if ($treatAsErr) { [void]$tfmReport.AppendLine("  TreatWarningsAsErrors: $treatAsErr") }
    [void]$tfmReport.AppendLine("")
}
Set-Content -Path (Join-Path $invDir "target-frameworks.txt") -Value $tfmReport.ToString()

Write-Host "Gathering Dependencies & Package References..."
$depReport = [System.Text.StringBuilder]::new()
[void]$depReport.AppendLine("# DEPENDENCY GRAPH & PACKAGE REFERENCES")
[void]$depReport.AppendLine("Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$depReport.AppendLine("")

foreach ($proj in $projects) {
    [xml]$xml = Get-Content $proj.FullName
    [void]$depReport.AppendLine("=== Project: $($proj.Name) ===")
    
    # Project References
    $projRefs = $xml.SelectNodes("//ProjectReference")
    if ($projRefs.Count -gt 0) {
        [void]$depReport.AppendLine("  Project References:")
        foreach ($pr in $projRefs) {
            $include = $pr.GetAttribute("Include")
            [void]$depReport.AppendLine("    - $include")
        }
    }
    
    # Package References
    $pkgRefs = $xml.SelectNodes("//PackageReference")
    if ($pkgRefs.Count -gt 0) {
        [void]$depReport.AppendLine("  Package References:")
        foreach ($pr in $pkgRefs) {
            $include = $pr.GetAttribute("Include")
            $version = $pr.GetAttribute("Version")
            $assets = $pr.GetAttribute("PrivateAssets")
            $str = "    - $include"
            if ($version) { $str += " (Version: $version)" }
            if ($assets) { $str += " [PrivateAssets: $assets]" }
            [void]$depReport.AppendLine($str)
        }
    }
    [void]$depReport.AppendLine("")
}
Set-Content -Path (Join-Path $invDir "dependencies.txt") -Value $depReport.ToString()

Write-Host "Gathering Source Generators & Analyzers Details..."
$genReport = [System.Text.StringBuilder]::new()
[void]$genReport.AppendLine("# SOURCE GENERATORS REGISTER")
[void]$genReport.AppendLine("Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$genReport.AppendLine("")
$genFiles = Get-ChildItem -Path "src/EricksonLopez.Mediator.Generator" -Recurse -Filter "*.cs"
foreach ($gf in $genFiles) {
    [void]$genReport.AppendLine("File: $($gf.Name)")
    [void]$genReport.AppendLine("  Path: $($gf.FullName)")
    [void]$genReport.AppendLine("  Length: $($gf.Length) bytes")
    $content = Get-Content $gf.FullName -Raw
    if ($content -match '\[Generator(.*?)\]') {
        [void]$genReport.AppendLine("  Type: Roslyn Incremental Generator ($($Matches[0]))")
    }
    if ($content -match 'class\s+(\w+)') {
        [void]$genReport.AppendLine("  Class: $($Matches[1])")
    }
    [void]$genReport.AppendLine("")
}
Set-Content -Path (Join-Path $invDir "source-generators.txt") -Value $genReport.ToString()

Write-Host "Gathering Analyzers & Diagnostic Descriptors..."
$anaReport = [System.Text.StringBuilder]::new()
[void]$anaReport.AppendLine("# ANALYZERS & DIAGNOSTIC DESCRIPTORS (ELM001-ELM011)")
[void]$anaReport.AppendLine("Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$anaReport.AppendLine("")

$diagFiles = Get-ChildItem -Path "src/EricksonLopez.Mediator.Generator" -Recurse -Filter "*.cs"
foreach ($df in $diagFiles) {
    $hasDiag = $false
    $fileLines = Get-Content $df.FullName
    $extracted = @()
    foreach ($line in $fileLines) {
        if ($line -match 'DiagnosticDescriptor|DiagnosticIds|ELM\d+') {
            $extracted += "  $($line.Trim())"
            $hasDiag = $true
        }
    }
    if ($hasDiag) {
        [void]$anaReport.AppendLine("File: $($df.Name)")
        foreach ($e in $extracted) {
            [void]$anaReport.AppendLine($e)
        }
        [void]$anaReport.AppendLine("")
    }
}
Set-Content -Path (Join-Path $invDir "analyzers.txt") -Value $anaReport.ToString()

Write-Host "Inventory script part 1 complete."
