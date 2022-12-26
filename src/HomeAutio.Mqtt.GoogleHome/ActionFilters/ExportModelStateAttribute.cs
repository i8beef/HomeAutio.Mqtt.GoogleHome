using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeAutio.Mqtt.GoogleHome.ActionFilters
{
    /// <summary>
    /// Export model state attribute.
    /// </summary>
    public class ExportModelStateAttribute : ModelStateTransfer
    {
        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Only export when ModelState is not valid
            if (!context.ModelState.IsValid)
            {
                // Export if we are redirecting
                if (context.Result is RedirectResult
                    or RedirectToRouteResult
                    or RedirectToActionResult)
                {
                    if (context.Controller is Controller controller && context.ModelState != null)
                    {
                        var modelState = SerialiseModelState(context.ModelState);
                        controller.TempData[Key] = modelState;
                    }
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
