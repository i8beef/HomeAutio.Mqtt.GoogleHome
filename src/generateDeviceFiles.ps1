$commandSchemaFile = $PSScriptRoot + "\smart-home-schema\schema\platform\commands.schema.json"
$deviceSchemaFile = $PSScriptRoot + "\smart-home-schema\schema\platform\types.schema.json"
$triatSchemaFile = $PSScriptRoot + "\smart-home-schema\schema\platform\traits.schema.json"

$commandJson = Get-Content -Path $commandSchemaFile | Out-String
$commandSchema = ConvertFrom-Json -InputObject $commandJson
$deviceJson = Get-Content -Path $deviceSchemaFile | Out-String
$deviceSchema = ConvertFrom-Json -InputObject $deviceJson
$traitJson = Get-Content -Path $triatSchemaFile | Out-String
$traitSchema = ConvertFrom-Json -InputObject $traitJson

## File generation
# Generate CommandTypes
$commands = $commandSchema.enum
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
Set-Content -Path $commandTypeFilePath -Value $commandTypesContent.Trim()

# Generate DeviceTypes
$devices = $deviceSchema.enum
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

$devices = $devices | Sort-Object
foreach ($device in $devices) {
    $deviceName = $device.Substring($device.LastIndexOf('.') + 1)
    $deviceTypeString = @"
,

        /// <summary>
        /// $($device).
        /// </summary>
        [EnumMember(Value = "$($device)")]
        $($deviceName)
"@

    $sb.Append($deviceTypeString)
}

$sb.AppendLine()
$sb.AppendLine($deviceTypesFooter)

$deviceTypesContent = $sb.ToString()
Set-Content -Path $deviceTypeFilePath -Value $deviceTypesContent.Trim()

# Generate TraitTypes
$traits = $traitSchema.enum
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

$traits = $traits | Sort-Object
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
Set-Content -Path $traitTypeFilePath -Value $traitTypesContent.Trim()