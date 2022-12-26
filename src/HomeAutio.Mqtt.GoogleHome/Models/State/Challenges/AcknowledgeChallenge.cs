using HomeAutio.Mqtt.GoogleHome.Models.Request;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.Challenges
{
    /// <summary>
    /// Acknowledge challenge.
    /// </summary>
    public class AcknowledgeChallenge : ChallengeBase
    {
        /// <inheritdoc />
        public override ChallengeType Type => ChallengeType.Acknowledge;

        /// <inheritdoc />
        public override string ChallengeNeededPhrase => "ackNeeded";

        /// <inheritdoc />
        public override string ChallengeRejectedPhrase => "userCancelled";

        /// <inheritdoc />
        public override bool Validate(Challenge challenge)
        {
            return challenge.Ack is not null && challenge.Ack.Value;
        }
    }
}
