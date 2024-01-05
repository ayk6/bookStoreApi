using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters
{
	public class ValidationFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var controller = context.RouteData.Values["controller"];
			var action = context.RouteData.Values["action"];

			//DTO
			var param = context.ActionArguments.SingleOrDefault(p=>
				p.Value.ToString().Contains("DTO")).Value;

			if (param is null) 
			{
				context.Result = new BadRequestObjectResult($"Object null. "+
					$"Controller : {controller}" + $"Action : {action}");
				return;
			}
			if (!context.ModelState.IsValid)
			{
				context.Result = new UnprocessableEntityObjectResult(context.ModelState);
			}
		}
	}
}
