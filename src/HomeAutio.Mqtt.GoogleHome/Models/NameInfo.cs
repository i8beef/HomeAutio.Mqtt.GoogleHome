using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Device name info.
    /// </summary>
    public class NameInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameInfo"/> class.
        /// </summary>
        public NameInfo()
        {
            DefaultNames = new List<string>();
            Nicknames = new List<string>();
        }

        /// <summary>
        /// Defaul names.
        /// </summary>
        public IList<string> DefaultNames { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nicknames.
        /// </summary>
        public IList<string> Nicknames { get; set; }
    }
}
