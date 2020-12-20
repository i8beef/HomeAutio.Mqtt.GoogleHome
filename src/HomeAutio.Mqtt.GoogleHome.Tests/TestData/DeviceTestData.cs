using HomeAutio.Mqtt.GoogleHome.Models.State;
using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Tests.TestData
{
    public static class DeviceTestData
    {
        public static Device FullDevice()
        {
            return new Device
            {
                Id = "test/device",
                Type = GoogleHome.Models.DeviceType.Light,
                Disabled = false,
                WillReportState = false,
                RoomHint = "Test",
                DeviceInfo = null,
                Name = new GoogleHome.Models.NameInfo
                {
                    DefaultNames = new List<string>(),
                    Name = "Test Device",
                    Nicknames = new List<string>()
                },
                Traits = new List<DeviceTrait>
                {
                    new DeviceTrait
                    {
                        Trait = GoogleHome.Models.TraitType.OnOff,
                        Commands = new Dictionary<string, IDictionary<string, string>>
                        {
                            {
                                "action.devices.commands.OnOff",
                                new Dictionary<string, string>
                                {
                                    {
                                        "on",
                                        "test/device/set"
                                    }
                                }
                            }
                        },
                        State = new Dictionary<string, DeviceState>
                        {
                            {
                                "on",
                                new DeviceState
                                {
                                    Topic = "/test/device",
                                    ValueMap = null
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
