$ErrorActionPreference = "Stop"

$transcriptPath = "C:\Users\erick\.gemini\antigravity-ide\brain\b9ff2cbc-b978-4672-baf2-5ccb83b28082\.system_generated\logs\transcript_full.jsonl"
if (-not (Test-Path $transcriptPath)) {
    Write-Error "Transcript not found at $transcriptPath"
    exit 1
}

Write-Host "Reading transcript lines from $transcriptPath..."
$lines = [System.IO.File]::ReadAllLines($transcriptPath)
Write-Host "Read $($lines.Length) lines."

$extractedFiles = [System.Collections.Generic.Dictionary[string, string]]::new()

foreach ($line in $lines) {
    if ($line.Contains('"name":"write_to_file"') -and $line.Contains('MEGA-AUDIT')) {
        try {
            $obj = $line | ConvertFrom-Json
            if ($obj.tool_calls) {
                foreach ($tc in $obj.tool_calls) {
                    if ($tc.name -eq "write_to_file" -and $tc.args) {
                        $targetFile = $tc.args.TargetFile
                        $codeContent = $tc.args.CodeContent
                        if ($targetFile -and ($targetFile -like "*MEGA-AUDIT*") -and ($codeContent -ne $null)) {
                            $extractedFiles[$targetFile] = $codeContent
                        }
                    }
                }
            }
        }
        catch {
            Write-Warning "Failed parsing line: $_"
        }
    }
}

Write-Host "Total unique files to extract: $($extractedFiles.Count)"

foreach ($kvp in $extractedFiles.GetEnumerator()) {
    $filePath = $kvp.Key
    $content = $kvp.Value
    
    $parentDir = [System.IO.Path]::GetDirectoryName($filePath)
    if (-not (Test-Path $parentDir)) {
        New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
    }
    
    [System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
    Write-Host "Extracted: $filePath ($($content.Length) chars)"
}

Write-Host "All audit deliverables extracted successfully."
