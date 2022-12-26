using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace HomeAutio.Mqtt.GoogleHome.ActionFilters
{
    /// <summary>
    /// Model state transfer.
    /// </summary>
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        /// <summary>
        /// Temp data key.
        /// </summary>
        protected const string Key = nameof(ModelStateTransfer);

        /// <summary>
        /// Serializes model state.
        /// </summary>
        /// <param name="modelState">The model state dictionary.</param>
        /// <returns>A serialized string.</returns>
        protected static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value?.AttemptedValue,
                    RawValue = kvp.Value?.RawValue,
                    ErrorMessages = kvp.Value is not null
                        ? kvp.Value.Errors.Select(err => err.ErrorMessage).ToList()
                        : new List<string>()
                });

            return JsonConvert.SerializeObject(errorList);
        }

        /// <summary>
        /// Deserializes model state.
        /// </summary>
        /// <param name="serialisedErrorList">Serialized error list.</param>
        /// <returns>A <see cref="ModelStateDictionary"/>.</returns>
        protected static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();

            if (errorList != null)
            {
                foreach (var item in errorList)
                {
                    var array = item.RawValue as Newtonsoft.Json.Linq.JArray;
                    var value = array?.ToObject<string[]>() ?? item.RawValue;

                    modelState.SetModelValue(item.Key, value, item.AttemptedValue);
                    foreach (var error in item.ErrorMessages)
                    {
                        modelState.AddModelError(item.Key, error);
                    }
                }
            }

            return modelState;
        }

        /// <summary>
        /// Model state transfer value class.
        /// </summary>
        public class ModelStateTransferValue
        {
            /// <summary>
            /// Key.
            /// </summary>
            public required string Key { get; init; }

            /// <summary>
            /// Attempted value.
            /// </summary>
            public string? AttemptedValue { get; init; }

            /// <summary>
            /// Raw value.
            /// </summary>
            public object? RawValue { get; init; }

            /// <summary>
            /// Error messages.
            /// </summary>
            public required ICollection<string> ErrorMessages { get; init; }
        }
    }
}
