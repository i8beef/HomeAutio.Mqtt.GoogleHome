$targetSchemaFiles = $PSScriptRoot + "\smart-home-schema\schema"

#
# Download latest schema files from GitHub
#
Function Download-SchemaFiles() {
    Write-Host "Downloading latest schema files: " -NoNewline

    $smartHomeSchemaUrl = "https://github.com/actions-on-google/smart-home-schema/archive/refs/heads/master.zip"
    $zip = $PSScriptRoot + "\schema.zip"
    $newSchemaFiles = $PSScriptRoot + "\smart-home-schema-master"

    Invoke-WebRequest $smartHomeSchemaUrl -Out $zip

    # Backup
    #$backupSchemaFiles = $PSScriptRoot + "\smart-home-schema\schema-backup"
    #Move-Item -Path $targetSchemaFiles -Destination $backupSchemaFiles

    # Unzip
    Expand-Archive $zip -Force -DestinationPath $PSScriptRoot
    Remove-Item -Recurse -Force -Path $targetSchemaFiles
    Move-Item -Path $newSchemaFiles -Destination $targetSchemaFiles
    
    Remove-Item $zip

    Write-Host "Done!" -ForegroundColor Green    
} 

#
# Generate CommandType.cs from schema
#
Function Generate-CommandTypeFile() {
    # Generate CommandTypes
    Write-Host "Generating CommandType.cs: " -NoNewline

    $commandSchemaFile = $targetSchemaFiles + "\platform\commands.schema.json"
    $commandJson = Get-Content -Path $commandSchemaFile | Out-String
    $commandSchema = ConvertFrom-Json -InputObject $commandJson
    $commands = $commandSchema.enum
    $commandTypeFilePath = $PSScriptRoot + "\HomeAutio.Mqtt.GoogleHome\Models\CommandType.cs"

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
    $sb.Append($commandTypesHeader) | Out-Null

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

        $sb.Append($commandTypeString) | Out-Null
    }

    $sb.AppendLine() | Out-Null
    $sb.AppendLine($commandTypesFooter) | Out-Null
    $commandTypesContent = $sb.ToString()

    Write-Host "Writing Commands.cs: " -NoNewline
    Set-Content -Path $commandTypeFilePath -Value $commandTypesContent.Trim()
    Write-Host "Done!" -ForegroundColor Green
}

#
# Generate DeviceType.cs from schema
#
Function Generate-DeviceTypeFile() {
    # Generate DeviceTypes
    Write-Host "Generating DeviceTypes.cs: " -NoNewline

    $deviceSchemaFile = $targetSchemaFiles + "\platform\types.schema.json"
    $deviceJson = Get-Content -Path $deviceSchemaFile | Out-String
    $deviceSchema = ConvertFrom-Json -InputObject $deviceJson
    $devices = $deviceSchema.enum
    $deviceTypeFilePath = $PSScriptRoot + "\HomeAutio.Mqtt.GoogleHome\Models\DeviceType.cs"

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
    $sb.Append($deviceTypesHeader) | Out-Null

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

        $sb.Append($deviceTypeString) | Out-Null
    }

    $sb.AppendLine() | Out-Null
    $sb.AppendLine($deviceTypesFooter) | Out-Null

    $deviceTypesContent = $sb.ToString()

    Set-Content -Path $deviceTypeFilePath -Value $deviceTypesContent.Trim()
    Write-Host "Done!" -ForegroundColor Green
}

#
# Generate TraitType.cs from schema
#
Function Generate-TraitTypeFile() {
    # Generate TraitTypes
    Write-Host "Generating TraitTypes.cs: " -NoNewline

    $triatSchemaFile = $targetSchemaFiles + "\platform\traits.schema.json"
    $traitJson = Get-Content -Path $triatSchemaFile | Out-String
    $traitSchema = ConvertFrom-Json -InputObject $traitJson
    $traits = $traitSchema.enum
    $traitTypeFilePath = $PSScriptRoot + "\HomeAutio.Mqtt.GoogleHome\Models\TraitType.cs"

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
    $sb.Append($traitTypesHeader) | Out-Null

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

        $sb.Append($traitTypeString) | Out-Null
    }

    $sb.AppendLine() | Out-Null
    $sb.AppendLine($traitTypesFooter) | Out-Null
    $traitTypesContent = $sb.ToString()

    Set-Content -Path $traitTypeFilePath -Value $traitTypesContent.Trim()
    Write-Host "Done!" -ForegroundColor Green
}

Write-Host "Updating from schema files"
Write-Host "=========================="

Download-SchemaFiles
Generate-CommandTypeFile
Generate-DeviceTypeFile
Generate-TraitTypeFile

Write-Host
Write-Host "Update finished" -ForegroundColor Green
