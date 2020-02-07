using System;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreProjectTemplate
{
    /// <summary>
    /// Attribute class for reading a property on an object and transforming it to an int.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IntPropertyConverterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The name of the property that holds the value.
        /// </summary>
        public string PropertyName { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelData = context.ActionArguments.FirstOrDefault().Value;

            if (modelData != null)
            {
                PropertyInfo p = modelData.GetType().GetProperty(PropertyName);
                if (p != null)
                {
                    string value = "";

                    try
                    {
                        value = p.GetValue(modelData, null).ToString();
                        var newValue = Convert.ToInt32(value) + 1;
                        p.SetValue(modelData, newValue);
                    }
                    catch (Exception e)
                    {
                        var logger = (ILogger<IntPropertyConverterAttribute>)context.HttpContext.RequestServices.GetService(typeof(ILogger<IntPropertyConverterAttribute>));
                        logger.LogError(e, "Unable to convert to int.");
                    }
                }
            }

            await next();
        }
    }
}
