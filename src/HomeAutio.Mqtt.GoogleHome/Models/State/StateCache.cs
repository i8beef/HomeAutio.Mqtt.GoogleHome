using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models.State
{
    public class StateCache : ConcurrentDictionary<string, string>
    {
        public StateCache(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
    }
}
