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
        public string Id { get; set; }

        /// <summary>
        /// Device type.
        /// </summary>
        [Required]
        public DeviceType Type { get; set; }

        /// <summary>
        /// Indicates if the device will report state.
        /// </summary>
        [Required]
        public bool WillReportState { get; set; }

        /// <summary>
        /// Room hint.
        /// </summary>
        [StringLength(255)]
        public string RoomHint { get; set; }

        /// <summary>
        /// Defaul names.
        /// </summary>
        [StringLength(1000)]
        public string DefaultNames { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Nicknames.
        /// </summary>
        [StringLength(1000)]
        public string Nicknames { get; set; }

        /// <summary>
        /// Manufacturer.
        /// </summary>
        [StringLength(255)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Model.
        /// </summary>
        [StringLength(255)]
        public string Model { get; set; }

        /// <summary>
        /// Hardware version.
        /// </summary>
        [StringLength(255)]
        public string HwVersion { get; set; }

        /// <summary>
        /// Software version.
        /// </summary>
        [StringLength(255)]
        public string SwVersion { get; set; }

        /// <summary>
        /// Trait configurations.
        /// </summary>
        public IEnumerable<TraitType> Traits { get; set; }
    }
}
