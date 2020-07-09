$traitsUrl = "https://developers.google.com/assistant/smarthome/traits"
$content = Invoke-WebRequest -Uri $traitsUrl

$table = $content.ParsedHtml.getElementsByTagName("table")[0]

$traits = @()
$commands = @()
$totalRows=@($table.rows).count
for ($idx = 1; $idx -lt $totalRows; $idx++) {
    $row = $table.rows[$idx]

    $traitName = $row.cells[0].innerText
    $traitId = $row.id
    $traitDescription = $row.cells[2].innerText

    Write-Host "Trait: $($traitName)"

    $traitUrl = "https://developers.google.com/assistant/smarthome/traits/$($traitName.ToLower())"
    $traitContent = Invoke-WebRequest -Uri $traitUrl
    $commandElementsH3 = $traitContent.ParsedHtml.getElementsByTagName("h3") |
        Where-Object { $_.innerText -ne $null -and $_.innerText.StartsWith("action.devices.commands.") } |
        Select-Object -expandproperty InnerText |
        Foreach-Object { $_.Trim() }
    $commandElementsH4 = $traitContent.ParsedHtml.getElementsByTagName("h4") |
        Where-Object { $_.innerText -ne $null -and $_.innerText.StartsWith("action.devices.commands.") } |
        Select-Object -expandproperty InnerText |
        Foreach-Object { $_.Trim() }
    $commandElementsTD = $traitContent.ParsedHtml.getElementsByTagName("td") |
        Where-Object { $_.innerText -ne $null -and $_.innerText.StartsWith("action.devices.commands.") } |
        Select-Object -expandproperty InnerText |
        Foreach-Object { $_.Trim() }

    $traitCommands = @()
    $traitCommands = $traitCommands + $commandElementsH3 | Select-Object -Unique
    $traitCommands = $traitCommands + $commandElementsH4 | Select-Object -Unique
    $traitCommands = $traitCommands + $commandElementsTD | Select-Object -Unique

    $trait = @{
        name = $traitName
        id = $traitId
        description = $traitDescription
    }

    $traits += $trait
    $commands = $commands + $traitCommands | Select-Object -Unique

    Write-Host "    Commands: $($traitCommands -join ", ")"
    Write-Host "===================="
}

# Generate TraitTypes
$traitTypeFilePath = ".\HomeAutio.Mqtt.GoogleHome\Models\TraitType.cs"

$traitTypesHeader = @"
using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Trait type enumeration.
    /// </summary>
    public enum TraitType
    {
"@

$traitTypesFooter = @"
        /// <summary>
        /// Unknown trait type.
        /// </summary>
        Unknown
    }
}
"@

$sb = [System.Text.StringBuilder]::new()
$sb.AppendLine($traitTypesHeader)

foreach ($trait in $traits) {
    $traitTypeString = @"
        /// <summary>
        /// $($trait.id).
        /// </summary>
        [EnumMember(Value = "$($trait.id)")]
        $($trait.name),

"@

    $sb.AppendLine($traitTypeString)
}

$sb.AppendLine($traitTypesFooter)
$traitTypesContent = $sb.ToString()
Set-Content -Path $traitTypeFilePath -Value $traitTypesContent.Trim()

# Generate CommandTypes
$commandTypeFilePath = ".\HomeAutio.Mqtt.GoogleHome\Models\CommandType.cs"

$commandTypesHeader = @"
using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device command types.
    /// </summary>
    public enum CommandType
    {
"@

$commandTypesFooter = @"
        /// <summary>
        /// Unknown command type.
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown
    }
}
"@

$sb = [System.Text.StringBuilder]::new()
$sb.AppendLine($commandTypesHeader)

$commands = $commands | Sort-Object
foreach ($command in $commands) {
    $commandName = $command.Substring($command.LastIndexOf('.') + 1)
    $commandName =  $commandName.Substring(0, 1).ToUpper() + $commandName.Substring(1)
    $commandTypeString = @"
        /// <summary>
        /// $($command)
        /// </summary>
        [EnumMember(Value = "$($command)")]
        $($commandName),

"@

    $sb.AppendLine($commandTypeString)
}

$sb.AppendLine($commandTypesFooter)
$commandTypesContent = $sb.ToString()
Set-Content -Path $commandTypeFilePath -Value $commandTypesContent.Trim()