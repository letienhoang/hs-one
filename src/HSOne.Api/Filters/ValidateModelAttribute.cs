using Microsoft.AspNetCore.Mvc.Filters;

namespace HSOne.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
