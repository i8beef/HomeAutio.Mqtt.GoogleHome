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
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Only export when ModelState is not valid
            if (!filterContext.ModelState.IsValid)
            {
                // Export if we are redirecting
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    var controller = filterContext.Controller as Controller;
                    if (controller != null && filterContext.ModelState != null)
                    {
                        var modelState = SerialiseModelState(filterContext.ModelState);
                        controller.TempData[Key] = modelState;
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
