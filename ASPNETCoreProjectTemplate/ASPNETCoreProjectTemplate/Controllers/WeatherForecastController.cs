using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreProjectTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [AllowAnonymous]
        [Route("{id}")]
        [HttpGet]
        public string Get(int id)
        {
            return id.ToString();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(TestException))]
        public IActionResult TestException()
        {
            throw new ApplicationException("Testing an exception being thrown by a controller");
        }

        [HttpGet]
        [Route(nameof(TestIntConverter))]
        public IActionResult TestIntConverter([IntConverter] string value)
        {
            return Ok(value);
        }

        [HttpPost]
        [Route(nameof(TestIntPropertyConverter))]
        [IntPropertyConverter(PropertyName = nameof(WeatherForecast.WindMph))]
        public IActionResult TestIntPropertyConverter(WeatherForecast model)
        {
            return Ok(model.WindMph);
        }

        [HttpPost]
        [Route(nameof(TestEventLoggerAttribute))]
        [EventLogger("TestEventLogger")]
        public IActionResult TestEventLoggerAttribute(string testArg, WeatherForecast model)
        {
            return Ok();
        }
    }
}
