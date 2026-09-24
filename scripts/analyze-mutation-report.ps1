$ErrorActionPreference = "Stop"

$jsonPath = "StrykerOutput/result/reports/mutation-report.json"
if (Test-Path $jsonPath) {
    $json = Get-Content $jsonPath -Raw | ConvertFrom-Json
    Write-Host "Stryker Schema Version: $($json.schemaVersion)"
    Write-Host "Thresholds: High=$($json.thresholds.high), Low=$($json.thresholds.low), Break=$($json.thresholds.break)"
    
    # Calculate totals
    $files = $json.files.PSObject.Properties
    $totalMutants = 0
    $killed = 0
    $survived = 0
    $noCoverage = 0
    $timeout = 0
    $compileErrors = 0
    $ignored = 0

    foreach ($f in $files) {
        $mutants = $f.Value.mutants
        foreach ($m in $mutants) {
            $totalMutants++
            switch ($m.status) {
                "Killed" { $killed++ }
                "Survived" { $survived++ }
                "NoCoverage" { $noCoverage++ }
                "Timeout" { $timeout++ }
                "CompileError" { $compileErrors++ }
                "Ignored" { $ignored++ }
            }
        }
    }

    $detected = $killed + $timeout
    $mutationScore = if ($totalMutants - $compileErrors - $ignored -gt 0) {
        [Math]::Round(($detected / ($totalMutants - $compileErrors - $ignored)) * 100, 2)
    } else { 0 }

    Write-Host "Total Mutants: $totalMutants"
    Write-Host "Killed: $killed"
    Write-Host "Survived: $survived"
    Write-Host "NoCoverage: $noCoverage"
    Write-Host "Timeout: $timeout"
    Write-Host "CompileErrors: $compileErrors"
    Write-Host "Ignored: $ignored"
    Write-Host "Mutation Score: $mutationScore %"

    if ($survived -gt 0) {
        Write-Host "`nSurvived Mutants Details:"
        foreach ($f in $files) {
            foreach ($m in $f.Value.mutants) {
                if ($m.status -eq "Survived") {
                    Write-Host "  File: $($f.Name)"
                    Write-Host "  Mutator: $($m.mutatorName) at line $($m.location.start.line):$($m.location.start.column)"
                    Write-Host "  Replacement: $($m.replacement)"
                    Write-Host "  Description: $($m.description)`n"
                }
            }
        }
    }
} else {
    Write-Host "No mutation report found."
}
