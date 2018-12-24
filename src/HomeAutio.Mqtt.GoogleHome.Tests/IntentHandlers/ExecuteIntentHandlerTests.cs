using System.Collections.Generic;
using System.Linq;
using Easy.MessageHub;
using HomeAutio.Mqtt.GoogleHome.IntentHandlers;
using HomeAutio.Mqtt.GoogleHome.Models.Request;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeAutio.Mqtt.GoogleHome.Tests.IntentHandlers
{
    public class ExecuteIntentHandlerTests
    {
        private readonly Mock<ILogger<ExecuteIntentHandler>> _logMock;
        private readonly Mock<IMessageHub> _messageHubMock;
        private readonly Mock<GoogleDeviceRepository> _deviceRepositoryMock;

        public ExecuteIntentHandlerTests()
        {
            _logMock = new Mock<ILogger<ExecuteIntentHandler>>();
            _messageHubMock = new Mock<IMessageHub>();

            var googleDeviceLogger = new Mock<ILogger<GoogleDeviceRepository>>();
            _deviceRepositoryMock = new Mock<GoogleDeviceRepository>(googleDeviceLogger.Object, "");
        }

        [Fact]
        public void ReturnsTransformedStateOnSuccess()
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
            var result = handler.Handle(intent);

            //Assert
            var commandResult = result.Commands.First();
            var colorState = (IDictionary<string, object>)commandResult.States["color"];

            // Transformed temperature
            Assert.True(colorState.ContainsKey("temperatureK"));

            // Tertiary level items
            var spectrumHSVState = (IDictionary<string, object>)colorState["spectrumHSV"];
            Assert.Equal(123, spectrumHSVState["hue"]);
            Assert.Equal(456, spectrumHSVState["saturation"]);
            Assert.Equal(789, spectrumHSVState["value"]);
        }
    }
}
