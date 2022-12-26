using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    /// <summary>
    /// Concurrent dictionary state cache.
    /// </summary>
    public class StateCache : ConcurrentDictionary<string, string?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateCache"/> class.
        /// </summary>
        /// <param name="dictionary">Initial state dictionary.</param>
        public StateCache(IDictionary<string, string?> dictionary)
            : base(dictionary)
        {
        }
    }
}
