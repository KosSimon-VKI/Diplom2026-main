$ErrorActionPreference = 'Stop'

$TargetPath = 'C:\Users\Admin\Desktop\for er.sql'
$Raw = [System.IO.File]::ReadAllText($TargetPath)

function Get-SqlTables {
    param([string]$Path)

    $tables = @{}
    $order = New-Object System.Collections.Generic.List[string]
    $current = $null

    foreach ($line in [System.IO.File]::ReadLines($Path)) {
        if ($line -match '^CREATE TABLE \[dbo\]\.\[(?<table>[^\]]+)\]\(') {
            $current = [pscustomobject]@{
                Name = $Matches.table
                Columns = New-Object System.Collections.Generic.List[object]
            }
            $tables[$current.Name] = $current
            [void]$order.Add($current.Name)
            continue
        }

        if ($null -eq $current) {
            continue
        }

        if ($line -match '^\)\s+ON\s+\[PRIMARY\]') {
            $current = $null
            continue
        }

        if ($line -match '^\s*\[(?<column>[^\]]+)\]\s+\[(?<baseType>[^\]]+)\](?<rest>.*)$') {
            $columnName = $Matches.column
            $type = $Matches.baseType
            $rest = $Matches.rest.Trim().TrimEnd(',')

            $typeArgsMatch = [regex]::Match($rest, '^\((?<args>[^)]*)\)')
            if ($typeArgsMatch.Success) {
                $type = "$type($($typeArgsMatch.Groups['args'].Value))"
            }

            $identityMatch = [regex]::Match($rest, 'IDENTITY\((?<identity>[^)]*)\)')
            if ($identityMatch.Success) {
                $type = "$type IDENTITY($($identityMatch.Groups['identity'].Value))"
            }

            $nullable = if ($rest -match '\bNOT NULL\b') { 'NOT NULL' } else { 'NULL' }

            [void]$current.Columns.Add([pscustomobject]@{
                Name = $columnName
                Type = $type
                Nullable = $nullable
            })
        }
    }

    return [pscustomobject]@{
        Tables = $tables
        Order = $order
    }
}

function Get-ForeignKeys {
    param([string]$Path)

    $foreignKeys = New-Object System.Collections.Generic.List[object]
    $pending = $null

    foreach ($line in [System.IO.File]::ReadLines($Path)) {
        if ($line -match '^ALTER TABLE \[dbo\]\.\[(?<table>[^\]]+)\].*FOREIGN KEY\(\[(?<column>[^\]]+)\]\)') {
            $pending = [pscustomobject]@{
                Table = $Matches.table
                Column = $Matches.column
                Constraint = $null
            }

            $constraintMatch = [regex]::Match($line, 'CONSTRAINT \[(?<constraint>[^\]]+)\]')
            if ($constraintMatch.Success) {
                $pending.Constraint = $constraintMatch.Groups['constraint'].Value
            }
            continue
        }

        if ($pending -and $line -match '^\s*REFERENCES \[dbo\]\.\[(?<target>[^\]]+)\] \(\[(?<targetColumn>[^\]]+)\]\)') {
            [void]$foreignKeys.Add([pscustomobject]@{
                FromTable = $pending.Table
                FromColumn = $pending.Column
                ToTable = $Matches.target
                ToColumn = $Matches.targetColumn
                Constraint = $pending.Constraint
            })
            $pending = $null
            continue
        }

        if ($line -match '^ALTER TABLE ') {
            $pending = $null
        }
    }

    return ,$foreignKeys
}

$parsed = Get-SqlTables -Path $TargetPath
$tables = $parsed.Tables
$subjectTables = @($parsed.Order | ForEach-Object { [string]$_ } | Where-Object { $_ -ne '__EFMigrationsHistory' })
$foreignKeys = @(Get-ForeignKeys -Path $TargetPath | Where-Object {
    $subjectTables -contains $_.FromTable -and $subjectTables -contains $_.ToTable
})

$builder = [System.Text.StringBuilder]::new()
[void]$builder.AppendLine('-- Clean DDL for ER diagram import.')
[void]$builder.AppendLine('-- Contains only subject tables and foreign-key relationships.')
[void]$builder.AppendLine()

foreach ($tableName in $subjectTables) {
    [void]$builder.AppendLine("CREATE TABLE $tableName (")
    $table = $tables[$tableName]
    $columns = @($table.Columns)
    for ($i = 0; $i -lt $columns.Count; $i++) {
        $column = $columns[$i]
        $comma = ','
        [void]$builder.AppendLine("    $($column.Name) $($column.Type) $($column.Nullable)$comma")
    }

    $pkColumn = if (($columns | Where-Object { $_.Name -eq 'Id' })) { 'Id' } elseif (($columns | Where-Object { $_.Name -eq 'id' })) { 'id' } else { $null }
    if ($pkColumn) {
        [void]$builder.AppendLine("    PRIMARY KEY ($pkColumn)")
    } else {
        $last = $builder.ToString()
        throw "Primary key column not found for $tableName"
    }
    [void]$builder.AppendLine(');')
    [void]$builder.AppendLine()
}

foreach ($fk in $foreignKeys) {
    if ($fk.Constraint) {
        [void]$builder.AppendLine("ALTER TABLE $($fk.FromTable) ADD CONSTRAINT $($fk.Constraint) FOREIGN KEY ($($fk.FromColumn)) REFERENCES $($fk.ToTable) ($($fk.ToColumn));")
    } else {
        [void]$builder.AppendLine("ALTER TABLE $($fk.FromTable) ADD FOREIGN KEY ($($fk.FromColumn)) REFERENCES $($fk.ToTable) ($($fk.ToColumn));")
    }
}

$encoding = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($TargetPath, $builder.ToString(), $encoding)

Write-Output "tables=$($subjectTables.Count)"
Write-Output "foreign_keys=$($foreignKeys.Count)"
