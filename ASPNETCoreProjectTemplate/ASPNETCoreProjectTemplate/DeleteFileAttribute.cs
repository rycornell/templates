using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ASPNETCoreProjectTemplate
{
    /// <summary>
    /// Attribute class for deleting a file after it is sent to the client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteFileAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultContext = await next();

            await resultContext.HttpContext.Response.CompleteAsync();
            var filePathResult = resultContext.Result as PhysicalFileResult;
            if (filePathResult != null)
            {
                if (File.Exists(filePathResult.FileName))
                {
                    File.Delete(filePathResult.FileName);
                }
            }
        }
    }
}
