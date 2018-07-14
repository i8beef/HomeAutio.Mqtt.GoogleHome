using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    public class NameInfo
    {
        public NameInfo()
        {
            DefaultNames = new List<string>();
            Nicknames = new List<string>();
        }

        public IList<string> DefaultNames { get; set; }
        public string Name { get; set; }
        public IList<string> Nicknames { get; set; }
    }
}
