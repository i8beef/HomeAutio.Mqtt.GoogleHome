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
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var serialisedModelState = controller?.TempData[Key] as string;

            if (serialisedModelState != null)
            {
                // Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    var modelState = DeserialiseModelState(serialisedModelState);
                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    // Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
