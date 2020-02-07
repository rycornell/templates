using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASPNETCoreProjectTemplate
{
    /// <summary>
    /// Attribute class for logging an event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventLoggerAttribute : ActionFilterAttribute
    {
        public string EventName { get; }

        public EventLoggerAttribute(string eventName)
        {
            EventName = eventName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var user = context.HttpContext.User.Identity.Name;
            var logEvent = new LogEvent(EventName) { User = user };

            foreach (var argument in context.ActionArguments)
            {
                logEvent.Fields.Add(argument.Key, JsonConvert.SerializeObject(argument.Value));
            }

            var result = resultContext.Result;

            if (resultContext.HttpContext.Response.StatusCode >= 200 && resultContext.HttpContext.Response.StatusCode <= 299)
            {
                logEvent.Successful = true;
                logEvent.Authorized = true;
            }
            else if (result is UnauthorizedResult || result is UnauthorizedObjectResult)
            {
                logEvent.Successful = false;
                logEvent.Authorized = false;
            }
            else
            {
                logEvent.Successful = false;
            }

            var eventLogger = (IEventLogger)context.HttpContext.RequestServices.GetService(typeof(IEventLogger));
            eventLogger.Log(logEvent);
        }
    }
}
