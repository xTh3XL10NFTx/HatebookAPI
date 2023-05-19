using Azure.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hatebook.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var apiError          = new ErrorResponse();
                apiError.StatusCode   = 422;
                apiError.StatusPhrase = "Unprocessable entity";
                apiError.Timestamp    = DateTime.Now;
                var errors            = context.ModelState.AsEnumerable();

                foreach( var error in errors )
                {
                    foreach(var inner in error.Value!.Errors )
                    {
                        apiError.Errors.Add( inner.ErrorMessage );
                    }
                }
                context.Result = new BadRequestObjectResult(apiError );
            }
        }
    }
}
