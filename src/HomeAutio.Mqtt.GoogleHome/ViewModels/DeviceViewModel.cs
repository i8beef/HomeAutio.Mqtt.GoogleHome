using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HomeAutio.Mqtt.GoogleHome.Models;

namespace HomeAutio.Mqtt.GoogleHome.ViewModels
{
    /// <summary>
    /// Device View Model.
    /// </summary>
    public class DeviceViewModel
    {
        /// <summary>
        /// Device id.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Id { get; init; }

        /// <summary>
        /// Device type.
        /// </summary>
        [Required]
        public required DeviceType Type { get; init; }

        /// <summary>
        /// Indicates if device is disabled or not.
        /// </summary>
        [Required]
        public bool Disabled { get; init; }

        /// <summary>
        /// Indicates if the device will report state.
        /// </summary>
        [Required]
        public bool WillReportState { get; init; }

        /// <summary>
        /// Room hint.
        /// </summary>
        [StringLength(255)]
        public string? RoomHint { get; init; }

        /// <summary>
        /// Defaul names.
        /// </summary>
        [StringLength(1000)]
        public string? DefaultNames { get; init; }

        /// <summary>
        /// Name.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; init; }

        /// <summary>
        /// Nicknames.
        /// </summary>
        [StringLength(1000)]
        public string? Nicknames { get; init; }

        /// <summary>
        /// Manufacturer.
        /// </summary>
        [StringLength(255)]
        public string? Manufacturer { get; init; }

        /// <summary>
        /// Model.
        /// </summary>
        [StringLength(255)]
        public string? Model { get; init; }

        /// <summary>
        /// Hardware version.
        /// </summary>
        [StringLength(255)]
        public string? HwVersion { get; init; }

        /// <summary>
        /// Software version.
        /// </summary>
        [StringLength(255)]
        public string? SwVersion { get; init; }

        /// <summary>
        /// Trait configurations.
        /// </summary>
        public IEnumerable<TraitType> Traits { get; init; } = new List<TraitType>();
    }
}
