using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeAutio.Mqtt.GoogleHome.ActionFilters
{
    /// <summary>
    /// Import model state attribute.
    /// </summary>
    public class ImportModelStateAttribute : ModelStateTransfer
    {
        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;

            if (controller?.TempData[Key] is string serialisedModelState)
            {
                // Only Import if we are viewing
                if (context.Result is ViewResult)
                {
                    var modelState = DeserialiseModelState(serialisedModelState);
                    context.ModelState.Merge(modelState);
                }
                else
                {
                    // Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
