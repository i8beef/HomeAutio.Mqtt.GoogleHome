namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// <see cref="DeviceType"/> to Material icon mapper.
    /// </summary>
    public static class DeviceTypeIconMapper
    {
        /// <summary>
        /// Maps a <see cref="DeviceType"/> to a known material icon name.
        /// </summary>
        /// <remarks>
        /// Sourced mapping from https://developers.home.google.com/use-cases
        /// </remarks>
        /// <param name="deviceType">The device type.</param>
        /// <returns>The material icon name.</returns>
        public static string Map(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.AC_UNIT => "air",
                DeviceType.AIRCOOLER => "ac_unit",
                DeviceType.AIRFRESHENER => "home_and_garden",
                DeviceType.AIRPURIFIER => "air_purifier_gen",
                DeviceType.AUDIO_VIDEO_RECEIVER => "settings_input_component",
                DeviceType.AWNING => "storefront",
                DeviceType.BATHTUB => "bathtub",
                DeviceType.BED => "bed",
                DeviceType.BLENDER => "blender",
                DeviceType.BLINDS => "blinds",
                DeviceType.BOILER => "water_heater",
                DeviceType.CAMERA => "nest_cam_outdoor",
                DeviceType.CARBON_MONOXIDE_DETECTOR => "co2",
                DeviceType.CHARGER => "charger",
                DeviceType.CLOSET => "dresser",
                DeviceType.COFFEE_MAKER => "coffee_maker",
                DeviceType.COOKTOP => "controller_gen",
                DeviceType.CURTAIN => "curtains",
                DeviceType.DEHUMIDIFIER => "air_purifier",
                DeviceType.DEHYDRATOR => "oven",
                DeviceType.DISHWASHER => "dishwasher_gen",
                DeviceType.DOOR => "meeting_room",
                DeviceType.DOORBELL => "doorbell_3p",
                DeviceType.DRAWER => "bottom_drawer",
                DeviceType.DRYER => "local_laundry_service",
                DeviceType.FAN => "mode_fan",
                DeviceType.FAUCET => "faucet",
                DeviceType.FIREPLACE => "fireplace",
                DeviceType.FREEZER => "kitchen",
                DeviceType.FRYER => "cooking",
                DeviceType.GARAGE => "garage_home",
                DeviceType.GATE => "gate",
                DeviceType.GRILL => "outdoor_grill",
                DeviceType.HEATER => "thermostat",
                DeviceType.HOOD => "range_hood",
                DeviceType.HUMIDIFIER => "humidity_mid",
                DeviceType.KETTLE => "kettle",
                DeviceType.LIGHT => "lightbulb",
                DeviceType.LOCK => "lock",
                DeviceType.MICROWAVE => "microwave_gen",
                DeviceType.MOP => "mop",
                DeviceType.MOWER => "grass",
                DeviceType.MULTICOOKER => "multicooker",
                DeviceType.NETWORK => "network_wifi",
                DeviceType.OUTLET => "outlet",
                DeviceType.OVEN => "oven",
                DeviceType.PERGOLA => "pergola",
                DeviceType.PETFEEDER => "pets",
                DeviceType.PRESSURECOOKER => "multicooker",
                DeviceType.RADIATOR => "insert_chart",
                DeviceType.REFRIGERATOR => "kitchen",
                DeviceType.REMOTECONTROL => "nest_remote",
                DeviceType.ROUTER => "router",
                DeviceType.SCENE => "device_unknown",
                DeviceType.SECURITYSYSTEM => "security",
                DeviceType.SENSOR => "sensors",
                DeviceType.SETTOP => "bento",
                DeviceType.SHOWER => "shower",
                DeviceType.SHUTTER => "vertical_shades_closed",
                DeviceType.SMOKE_DETECTOR => "detector_smoke",
                DeviceType.SOUNDBAR => "surround_sound",
                DeviceType.SOUSVIDE => "set_meal",
                DeviceType.SPEAKER => "speaker",
                DeviceType.SPRINKLER => "sprinkler",
                DeviceType.STANDMIXER => "cooking",
                DeviceType.STREAMING_BOX => "view_stream",
                DeviceType.STREAMING_SOUNDBAR => "surround_sound",
                DeviceType.STREAMING_STICK => "chromecast_device",
                DeviceType.SWITCH => "switch",
                DeviceType.THERMOSTAT => "nest_thermostat_zirconium_eu",
                DeviceType.TV => "tv",
                DeviceType.VACUUM => "vacuum",
                DeviceType.VALVE => "valve",
                DeviceType.WASHER => "local_laundry_service",
                DeviceType.WATERHEATER => "water_heater",
                DeviceType.WATERPURIFIER => "water_drop",
                DeviceType.WATERSOFTENER => "water_drop",
                DeviceType.WINDOW => "window",
                DeviceType.YOGURTMAKER => "multicooker",
                DeviceType.Unknown => "device_unknown",
                _ => "device_unknown"
            };
        }
    }
}
