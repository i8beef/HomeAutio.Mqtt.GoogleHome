using System.ComponentModel.DataAnnotations;
using HomeAutio.Mqtt.GoogleHome.Models;
using HomeAutio.Mqtt.GoogleHome.Models.State.Challenges;

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
        public required TraitType Trait { get; init; }

        /// <summary>
        /// Attributes.
        /// </summary>
        public string? Attributes { get; init; }

        /// <summary>
        /// Commands.
        /// </summary>
        public string? Commands { get; init; }

        /// <summary>
        /// State.
        /// </summary>
        public string? State { get; init; }

        /// <summary>
        /// Challenge type.
        /// </summary>
        public ChallengeType ChallengeType { get; init; } = ChallengeType.None;

        /// <summary>
        /// Challenge pin, if applicable.
        /// </summary>
        public string? ChallengePin { get; init; }
    }
}
