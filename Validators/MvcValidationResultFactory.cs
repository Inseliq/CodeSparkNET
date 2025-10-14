using System.Linq;
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

                var model = context.ActionArguments.Values.FirstOrDefault();

                return controller.View(model);
            }

            return new BadRequestObjectResult(validationProblemDetails);
        }
    }
}
