using System.ComponentModel.DataAnnotations;
using HomeAutio.Mqtt.GoogleHome.Models;

namespace HomeAutio.Mqtt.GoogleHome.ViewModels
{
    /// <summary>
    /// Trait View Model.
    /// </summary>
    public class TraitViewModel
    {
        /// <summary>
        /// Device id.
        /// </summary>
        [Required]
        public TraitType Trait { get; set; }

        /// <summary>
        /// Attributes.
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// Commands.
        /// </summary>
        public string Commands { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public string State { get; set; }
    }
}
