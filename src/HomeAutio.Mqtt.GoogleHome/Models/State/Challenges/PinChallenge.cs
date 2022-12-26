using HomeAutio.Mqtt.GoogleHome.Models.Request;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.Challenges
{
    /// <summary>
    /// Pin challenge.
    /// </summary>
    public class PinChallenge : ChallengeBase
    {
        /// <inheritdoc />
        public override ChallengeType Type => ChallengeType.Pin;

        /// <summary>
        /// Pin.
        /// </summary>
        public required string Pin { get; init; }

        /// <inheritdoc />
        public override string ChallengeNeededPhrase => "pinNeeded";

        /// <inheritdoc />
        public override string ChallengeRejectedPhrase => "pinIncorrect";

        /// <inheritdoc />
        public override bool Validate(Challenge challenge)
        {
            return challenge.Pin is not null && challenge.Pin == Pin;
        }
    }
}
