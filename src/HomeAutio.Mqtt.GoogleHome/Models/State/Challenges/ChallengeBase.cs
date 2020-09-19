using HomeAutio.Mqtt.GoogleHome.Models.Request;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.Challenges
{
    /// <summary>
    /// Challenge base class.
    /// </summary>
    public abstract class ChallengeBase
    {
        /// <summary>
        /// Map type.
        /// </summary>
        public abstract ChallengeType Type { get; }

        /// <summary>
        /// Challenge phrase.
        /// </summary>
        [JsonIgnore]
        public abstract string ChallengeNeededPhrase { get; }

        /// <summary>
        /// Challenge phrase.
        /// </summary>
        [JsonIgnore]
        public abstract string ChallengeRejectedPhrase { get; }

        /// <summary>
        /// Validate a Challenge.
        /// </summary>
        /// <param name="challenge">Challenge to verify.</param>
        /// <returns><c>true</c> if challenge passed, otherwise <c>false</c>.</returns>
        public abstract bool Validate(Challenge challenge);
    }
}
