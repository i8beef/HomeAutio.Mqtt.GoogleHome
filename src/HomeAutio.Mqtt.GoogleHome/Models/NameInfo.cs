using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device name info.
    /// </summary>
    public class NameInfo
    {
        /// <summary>
        /// Defaul names.
        /// </summary>
        public IList<string>? DefaultNames { get; init; }

        /// <summary>
        /// Name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Nicknames.
        /// </summary>
        public IList<string>? Nicknames { get; init; }
    }
}
