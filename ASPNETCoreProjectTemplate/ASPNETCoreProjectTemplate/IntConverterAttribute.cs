using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreProjectTemplate
{
    /// <summary>
    /// Attribute class for reading a route parameter and transforming it to an int.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class IntConverterAttribute : ModelBinderAttribute, IModelBinder
    {
        public IntConverterAttribute() : base(typeof(IntConverterAttribute))
        {
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var newValue = Convert.ToInt32(value) + 1;
                bindingContext.Result = ModelBindingResult.Success(newValue);
            }
            catch (Exception e)
            {
                var logger = (ILogger<IntConverterAttribute>)bindingContext.HttpContext.RequestServices.GetService(typeof(ILogger<IntConverterAttribute>));
                logger.LogError(e, "Unable to convert to int.");
            }

            return Task.CompletedTask;
        }
    }
}
