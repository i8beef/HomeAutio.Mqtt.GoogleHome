namespace HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps
{
    public abstract class MapBase
    {
        public MapType Type { get; set; }
        public object Google { get; set; }

        public abstract bool MatchesMqtt(string value);
        public bool MatchesGoogle(object value)
        {
            return value.Equals(Google);
        }

        public abstract string ConvertToMqtt(object value);
        public object ConvertToGoogle(string value)
        {
            return Google;
        }
    }
}
