using System.Runtime.Serialization;

namespace HomeAutio.Mqtt.GoogleHome.Models.State.Challenges
{
    /// <summary>
    /// Challenge type.
    /// </summary>
    public enum ChallengeType
    {
        /// <summary>
        /// None.
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Acknowledge.
        /// </summary>
        [EnumMember(Value = "ack")]
        Acknowledge,

        /// <summary>
        /// Pin.
        /// </summary>
        [EnumMember(Value = "pin")]
        Pin
    }
}
