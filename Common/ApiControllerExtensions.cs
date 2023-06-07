namespace Hatebook.Common
{
    public static class ApiControllerExtensions
    {
        public static IActionResult? ValidateModelState(this ControllerBase controller)
        {
            if (!controller.ModelState.IsValid)
            {
                return new BadRequestObjectResult(controller.ModelState);
            }
            return null;
        }
    }
}
