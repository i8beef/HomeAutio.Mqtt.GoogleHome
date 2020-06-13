$traitUrl = "https://developers.google.com/assistant/smarthome/traits"
$content = Invoke-WebRequest -Uri $traitUrl

$table = $content.ParsedHtml.getElementsByTagName("table")[0]

$traits = @()
$totalRows=@($table.rows).count
for ($idx = 1; $idx -lt $totalRows; $idx++) {
    $row = $table.rows[$idx]
    $cells = @($row.cells)        

    $traitName = $row.cells[0].innerText
    $traitId = $row.id
    $traitDescription = $row.cells[2].innerText

    $trait = @{
        name = $traitName
        id = $traitId
        description = $traitDescription
    }

    $traits += $trait
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

Set-Content -Path $traitTypeFilePath -Value $traitTypesContent