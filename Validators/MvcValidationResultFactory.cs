using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace CodeSparkNET.Infrastructure
{
    public class MvcValidationResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
        {
            if (context.Controller is Controller controller)
            {
                if (validationProblemDetails?.Errors != null)
                {
                    foreach (var kv in validationProblemDetails.Errors)
                    {
                        foreach (var msg in kv.Value)
                        {
                            controller.ModelState.AddModelError(kv.Key ?? string.Empty, msg);
                        }
                    }
                }

                // Detect AJAX / JSON request:
                var request = context.HttpContext.Request;
                var isAjax = request.Headers.TryGetValue("X-Requested-With", out var xrw) && xrw == "XMLHttpRequest";
                var accept = request.Headers["Accept"].ToString();
                var wantsJson = !string.IsNullOrEmpty(accept) && accept.Contains("application/json");

                if (isAjax || wantsJson)
                {
                    // Build a friendly JSON payload with errors
                    var errors = new Dictionary<string, string[]>();
                    if (validationProblemDetails?.Errors != null)
                    {
                        foreach (var kv in validationProblemDetails.Errors)
                        {
                            errors[kv.Key ?? string.Empty] = kv.Value.ToArray();
                        }
                    }
                    else
                    {
                        // fallback: read from ModelState
                        foreach (var kv in controller.ModelState)
                        {
                            var errs = kv.Value.Errors.Select(e => e.ErrorMessage).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                            if (errs.Length > 0) errors[kv.Key] = errs;
                        }
                    }

                    var payload = new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errors
                    };

                    return new JsonResult(payload) { StatusCode = 400 };
                }

                // Fallback: existing behavior for normal form submissions
                var model = context.ActionArguments.Values.FirstOrDefault();
                return controller.View(model);
            }

            return new BadRequestObjectResult(validationProblemDetails);
        }
    }
}
