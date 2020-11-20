using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using HomeAutio.Mqtt.GoogleHome.Models.State.ValueMaps;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.IntentHandlers
{
    public class ExecuteIntentHandlerTests
    {
        private readonly Mock<ILogger<ExecuteIntentHandler>> _logMock;
        private readonly Mock<IMessageHub> _messageHubMock;
        private readonly Mock<IGoogleDeviceRepository> _deviceRepositoryMock;

        public ExecuteIntentHandlerTests()
        {
            _logMock = new Mock<ILogger<ExecuteIntentHandler>>();
            _messageHubMock = new Mock<IMessageHub>();

            _deviceRepositoryMock = new Mock<IGoogleDeviceRepository>();
        }

        [Fact]
        public async Task ReturnsTransformedStateOnSuccess()
        {
            // Arrange
            var commandParams = new Dictionary<string, object>
            {
                {
                    "color",
                    new Dictionary<string, object>
                    {
                        { "temperature", 85 },
                        {
                            "spectrumHSV",
                            new Dictionary<string, object>
                            {
                                { "hue", 123 },
                                { "saturation", 456 },
                                { "value", 789 },
                            }
                        }
                    }
                }
            };

            var intent = new ExecuteIntent
            {
                Payload = new ExecuteIntentPayload
                {
                    Commands = new List<Command>
                    {
                        new Command
                        {
                            Devices = new List<Device>
                            {
                                new Device { Id = "test/testLight" }
                            },
                            Execution = new List<Execution>
                            {
                                new Execution
                                {
                                    Command = "action.devices.commands.ColorAbsolute",
                                    Params = commandParams
                                }
                            }
                        }
                    }
                }
            };

            var handler = new ExecuteIntentHandler(_logMock.Object, _messageHubMock.Object, _deviceRepositoryMock.Object);

            // Act
            var result = await handler.Handle(intent);

            //Assert
            var commandResult = result.Commands.First();
            var colorState = (IDictionary<string, object>)commandResult.States["color"];

            // Transformed temperature
            Assert.True(colorState.ContainsKey("temperatureK"));

            // Tertiary level items
            var spectrumHSVState = (IDictionary<string, object>)colorState["spectrumHsv"];
            Assert.Equal(123, spectrumHSVState["hue"]);
            Assert.Equal(456, spectrumHSVState["saturation"]);
            Assert.Equal(789, spectrumHSVState["value"]);
        }

        [Fact]
        public async Task ReturnsCameraStreamStateOnSuccess()
        {
            // Arrange
            var commandParams = new Dictionary<string, object>
            {
                { "StreamToChromecast", true },
                { "SupportedStreamProtocols", new List<string> { "hls" } }
            };

            var intent = new ExecuteIntent
            {
                Payload = new ExecuteIntentPayload
                {
                    Commands = new List<Command>
                    {
                        new Command
                        {
                            Devices = new List<Device>
                            {
                                new Device { Id = "test/camera" }
                            },
                            Execution = new List<Execution>
                            {
                                new Execution
                                {
                                    Command = "action.devices.commands.GetCameraStream",
                                    Params = commandParams
                                }
                            }
                        }
                    }
                }
            };

            var device = new GoogleHome.Models.State.Device
            {
                Id = "test/camera",
                Traits = new List<GoogleHome.Models.State.DeviceTrait>
                {
                    new GoogleHome.Models.State.DeviceTrait
                    {
                       Trait = GoogleHome.Models.TraitType.CameraStream,
                       Commands = new Dictionary<string, IDictionary<string, string>>
                       {
                           { "action.devices.commands.GetCameraStream", new Dictionary<string, string>() }
                       },
                       State = new Dictionary<string, GoogleHome.Models.State.DeviceState>
                       {
                           {
                               "cameraStreamAccessUrl",
                               new GoogleHome.Models.State.DeviceState { 
                                   GoogleType = GoogleHome.Models.State.GoogleType.String,
                                   ValueMap = new List<MapBase>
                                   {
                                       new StaticMap { Google = "https://fluffysheep.com/baaaaa.mp4", Type = MapType.Static }
                                   }
                               } 
                           },
                           {
                               "cameraStreamReceiverAppId",
                               new GoogleHome.Models.State.DeviceState {
                                   GoogleType = GoogleHome.Models.State.GoogleType.String,
                                   ValueMap = new List<MapBase>
                                   {
                                       new StaticMap { Google = "1g2f89213hg", Type = MapType.Static }
                                   }
                               }
                           },
                           {
                               "cameraStreamAuthToken",
                               new GoogleHome.Models.State.DeviceState {
                                   GoogleType = GoogleHome.Models.State.GoogleType.String,
                                   ValueMap = new List<MapBase>
                                   {
                                       new StaticMap { Google = "12657342190192783", Type = MapType.Static }
                                   }
                               }
                           },
                           {
                               "cameraStreamProtocol",
                               new GoogleHome.Models.State.DeviceState {
                                   GoogleType = GoogleHome.Models.State.GoogleType.String,
                                   ValueMap = new List<MapBase>
                                   {
                                       new StaticMap { Google = "progressive_mp4", Type = MapType.Static }
                                   }
                               }
                           }
                       }
                    }
                }
            };

            // Camera state
            _deviceRepositoryMock.Setup(x => x.Get(It.IsAny<string>()))
                .Returns(device);

            var handler = new ExecuteIntentHandler(_logMock.Object, _messageHubMock.Object, _deviceRepositoryMock.Object);

            // Act
            var result = await handler.Handle(intent);

            //Assert            
            var commandResult = result.Commands.First();
            Assert.Equal("https://fluffysheep.com/baaaaa.mp4", commandResult.States["cameraStreamAccessUrl"]);
            Assert.Equal("1g2f89213hg", commandResult.States["cameraStreamReceiverAppId"]);
            Assert.Equal("12657342190192783", commandResult.States["cameraStreamAuthToken"]);
            Assert.Equal("progressive_mp4", commandResult.States["cameraStreamProtocol"]);
        }
    }
}
