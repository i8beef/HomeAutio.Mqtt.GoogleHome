using System.Collections.Generic;

namespace HomeAutio.Mqtt.GoogleHome.Models
{
    /// <summary>
    /// Command to state key mapper.
    /// </summary>
    public static class CommandToStateKeyMapper
    {
        /// <summary>
        /// Maps a command parameter key to a state key.
        /// </summary>
        /// <remarks>
        /// This is possibly the most ill advised implementation in this whole project...
        /// </remarks>
        /// <param name="commandParameterKey">The command parameter key to map.</param>
        /// <returns>The state key.</returns>
        public static string Map(string commandParameterKey)
        {
            if (string.IsNullOrEmpty(commandParameterKey))
            {
                return commandParameterKey;
            }

            var replacements = new Dictionary<string, string>
            {
                { "arm", "isArmed" },
                { "armLevel", "currentArmLevel" },
                { "color.spectrumHSV*", "color.spectrumHsv" },
                { "color.spectrumRGB", "color.spectrumRgb" },
                { "color.temperature", "color.temperatureK" },
                { "fanSpeed", "currentFanSpeedSetting" },
                { "fanSpeedPercent", "currentFanSpeedPercent" },
                { "lock", "isLocked" },
                { "mute", "isMuted" },
                { "newApplication", "currentApplication" },
                { "newApplicationName", "currentApplication" },
                { "pause", "isPaused" },
                { "start", "isRunning" },
                { "timerTimeSec", "timerRemainingSec" },
                { "updateModeSettings*", "currentModeSettings" },
                { "updateToggleSettings*", "currentToggleSettings" },
                { "volumeLevel", "currentVolume" }
            };

            foreach (var replacement in replacements)
            {
                if (replacement.Key.EndsWith('*'))
                {
                    // Handle starts with replacements
                    var replacementBase = replacement.Key.TrimEnd('*');
                    if (commandParameterKey.StartsWith(replacementBase))
                    {
                        return $"{replacement.Value}{commandParameterKey.Substring(replacementBase.Length)}";
                    }
                }
                else
                {
                    // Handle regular equality replacements
                    if (commandParameterKey == replacement.Key)
                    {
                        return $"{replacement.Value}{commandParameterKey.Substring(replacement.Key.Length)}";
                    }
                }
            }

            return commandParameterKey;
        }
    }
}
