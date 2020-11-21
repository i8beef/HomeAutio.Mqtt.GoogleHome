Install-Module powershell-yaml -Scope CurrentUser

$yamlRoot = $PSScriptRoot + "\smart-home-schema\schema\types"
$yamlDeviceFiles = Get-ChildItem -Path $yamlRoot -Recurse -Filter "examples.yaml"

$devices = @()
$traits = @()
$commands = @()
foreach ($file in $yamlDeviceFiles) {
    Write-Host $file.FullName
    $yaml = Get-Content -Path $file.FullName | Out-String
    $obj = ConvertFrom-Yaml -Yaml $yaml

    $devices += $obj
    $traits += $obj.traits
    if ($null -ne $obj.commands) {
        $commands += $obj.commands.Keys
    }
}

$traits = $traits | Select -Unique | Sort
$traits

$commands = $commands | Select -Unique | Sort
$commands

## File generation
# Generate DeviceTypes
$deviceTypeFilePath = ".\HomeAutio.Mqtt.GoogleHome\Models\DeviceType.cs"

$deviceTypesHeader = @"
using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device type enumeration.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown
"@

$deviceTypesFooter = @"
    }
}
"@

$sb = [System.Text.StringBuilder]::new()
$sb.Append($deviceTypesHeader)

foreach ($device in $devices) {
    $deviceName = (Get-Culture).TextInfo.ToTitleCase($device.name)
    $deviceName = $deviceName -Replace "Simple ", "" 
    $deviceName = $deviceName -Replace " ", ""
    $deviceTypeString = @"
,

        /// <summary>
        /// $($device.type).
        /// </summary>
        [EnumMember(Value = "$($device.type)")]
        $($deviceName)
"@

    $sb.Append($deviceTypeString)
}

$sb.AppendLine()
$sb.AppendLine($deviceTypesFooter)

$sb.ToString()
#Set-Content -Path $deviceTypeFilePath -Value $sb.ToString()

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
        /// <summary>
        /// Unknown trait type.
        /// </summary>
        Unknown
"@

$traitTypesFooter = @"
    }
}
"@

$sb = [System.Text.StringBuilder]::new()
$sb.Append($traitTypesHeader)

foreach ($trait in $traits) {
    $traitName = $trait -Replace "action.devices.traits.", ""
    $traitTypeString = @"
,

        /// <summary>
        /// $($trait).
        /// </summary>
        [EnumMember(Value = "$($trait)")]
        $($traitName)
"@

    $sb.Append($traitTypeString)
}

$sb.AppendLine()
$sb.AppendLine($traitTypesFooter)
$traitTypesContent = $sb.ToString()
$traitTypesContent
#Set-Content -Path $traitTypeFilePath -Value $traitTypesContent.Trim()

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
        /// <summary>
        /// Unknown command type.
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown
"@

$commandTypesFooter = @"
    }
}
"@

$sb = [System.Text.StringBuilder]::new()
$sb.Append($commandTypesHeader)

$commands = $commands | Sort-Object
foreach ($command in $commands) {
    $commandName = $command.Substring($command.LastIndexOf('.') + 1)
    $commandName =  $commandName.Substring(0, 1).ToUpper() + $commandName.Substring(1)
    $commandTypeString = @"
,

        /// <summary>
        /// $($command)
        /// </summary>
        [EnumMember(Value = "$($command)")]
        $($commandName)
"@

    $sb.Append($commandTypeString)
}

$sb.AppendLine()
$sb.AppendLine($commandTypesFooter)
$commandTypesContent = $sb.ToString()
$commandTypesContent
#Set-Content -Path $commandTypeFilePath -Value $commandTypesContent.Trim()