$ErrorActionPreference = "Stop"

$invDir = "MEGA-AUDIT/evidence/inventory"
$typesOutput = [System.Text.StringBuilder]::new()
$apiOutput = [System.Text.StringBuilder]::new()

[void]$typesOutput.AppendLine("# TYPE SYSTEM INVENTORY (PUBLIC, INTERNAL, INTERFACES, STRUCTS, RECORDS)")
[void]$typesOutput.AppendLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$typesOutput.AppendLine("")

[void]$apiOutput.AppendLine("# COMPLETE PUBLIC API SURFACE")
[void]$apiOutput.AppendLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$apiOutput.AppendLine("")

$srcDirs = Get-ChildItem -Path "src" -Directory
foreach ($sd in $srcDirs) {
    $dlls = Get-ChildItem -Path $sd.FullName -Recurse -Filter "$($sd.Name).dll" | 
            Where-Object { $_.FullName -like "*bin\Release\net10.0*" -or $_.FullName -like "*bin\Release\netstandard2.0*" -or $_.FullName -like "*bin\Debug\net10.0*" -or $_.FullName -like "*bin\Debug\netstandard2.0*" }
    
    if ($dlls.Count -eq 0) {
        $dlls = Get-ChildItem -Path $sd.FullName -Recurse -Filter "$($sd.Name).dll" | 
                Where-Object { $_.FullName -like "*bin\Release*" -or $_.FullName -like "*bin\Debug*" }
    }

    if ($dlls.Count -gt 0) {
        $dll = $dlls[0]
        [void]$typesOutput.AppendLine("================================================================================")
        [void]$typesOutput.AppendLine("ASSEMBLY: $($sd.Name)")
        [void]$typesOutput.AppendLine("PATH: $($dll.FullName)")
        [void]$typesOutput.AppendLine("================================================================================")
        
        [void]$apiOutput.AppendLine("================================================================================")
        [void]$apiOutput.AppendLine("ASSEMBLY: $($sd.Name)")
        [void]$apiOutput.AppendLine("================================================================================")

        try {
            $asm = [System.Reflection.Assembly]::LoadFrom($dll.FullName)
            $types = $asm.GetTypes() | Sort-Object FullName
            
            foreach ($t in $types) {
                $visibility = if ($t.IsPublic -or $t.IsNestedPublic) { "public" } else { "internal" }
                $kind = "class"
                if ($t.IsInterface) { $kind = "interface" }
                elseif ($t.IsValueType -and -not $t.IsEnum) { $kind = "struct" }
                elseif ($t.IsEnum) { $kind = "enum" }
                elseif ($t.BaseType.Name -eq "MulticastDelegate") { $kind = "delegate" }

                $interfaces = ($t.GetInterfaces() | ForEach-Object { $_.Name }) -join ", "
                $ifaceStr = if ($interfaces) { " : $interfaces" } else { "" }

                [void]$typesOutput.AppendLine("[$visibility] $kind $($t.FullName)$ifaceStr")
                
                # Attributes
                $attrs = $t.GetCustomAttributesData() | ForEach-Object { $_.AttributeType.Name }
                if ($attrs.Count -gt 0) {
                    [void]$typesOutput.AppendLine("    Attributes: $($attrs -join ', ')")
                }

                if ($visibility -eq "public") {
                    [void]$apiOutput.AppendLine("$kind $($t.FullName)$ifaceStr")
                    # Constructors
                    foreach ($c in ($t.GetConstructors() | Sort-Object ToString)) {
                        [void]$apiOutput.AppendLine("    $($c.ToString())")
                    }
                    # Methods
                    foreach ($m in ($t.GetMethods([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly') | Sort-Object Name)) {
                        if (-not $m.IsSpecialName) {
                            [void]$apiOutput.AppendLine("    $($m.ToString())")
                        }
                    }
                    # Properties
                    foreach ($p in ($t.GetProperties([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly') | Sort-Object Name)) {
                        [void]$apiOutput.AppendLine("    property $($p.PropertyType.Name) $($p.Name)")
                    }
                    # Fields
                    foreach ($f in ($t.GetFields([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly') | Sort-Object Name)) {
                        [void]$apiOutput.AppendLine("    field $($f.FieldType.Name) $($f.Name)")
                    }
                    [void]$apiOutput.AppendLine("")
                }
            }
        }
        catch {
            [void]$typesOutput.AppendLine("ERROR inspecting $($sd.Name): $_")
            [void]$apiOutput.AppendLine("ERROR inspecting $($sd.Name): $_")
        }
        [void]$typesOutput.AppendLine("")
    }
}

Set-Content -Path (Join-Path $invDir "types.txt") -Value $typesOutput.ToString()
Set-Content -Path (Join-Path $invDir "public-api.txt") -Value $apiOutput.ToString()
Write-Host "Extracted types.txt and public-api.txt successfully."
