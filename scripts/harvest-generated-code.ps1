$ErrorActionPreference = "Stop"

$invDir = "MEGA-AUDIT/evidence/inventory"
$genOutput = [System.Text.StringBuilder]::new()

[void]$genOutput.AppendLine("# GENERATED CODE ARTIFACTS & ROSLYN EMITTED DISPATCHERS")
[void]$genOutput.AppendLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$genOutput.AppendLine("")

$gFiles = Get-ChildItem -Path "tests", "samples", "src" -Recurse -Filter "*.g.cs"
[void]$genOutput.AppendLine("Total generated files found: $($gFiles.Count)")
[void]$genOutput.AppendLine("")

foreach ($gf in $gFiles) {
    [void]$genOutput.AppendLine("================================================================================")
    [void]$genOutput.AppendLine("FILE: $($gf.FullName)")
    [void]$genOutput.AppendLine("SIZE: $($gf.Length) bytes")
    [void]$genOutput.AppendLine("================================================================================")
    $content = Get-Content $gf.FullName -Raw
    [void]$genOutput.AppendLine($content)
    [void]$genOutput.AppendLine("")
}

Set-Content -Path (Join-Path $invDir "generated-code.txt") -Value $genOutput.ToString()
Write-Host "Generated generated-code.txt successfully with $($gFiles.Count) files."
