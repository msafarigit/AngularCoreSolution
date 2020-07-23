using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Infrastructure.Logging;

namespace AngularCore.Controllers
{
    public class WeatherForecastController : ApiControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILoggerService _loggerService;
        private readonly AuthenticationOptions _authenticationOptions;

        public WeatherForecastController(IConfiguration configuration, ILoggerService loggerService, IOptions<AuthenticationOptions> authenticationOptions)
        {
            _loggerService = loggerService;
            _authenticationOptions = authenticationOptions.Value;

            //Raw Configuration Value Binding
            string x = configuration["Authentication:Username"];
            string y = configuration["Authentication:BCS:Username"];
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Random rng = new Random();
            return Enumerable.Range(1, 5)
                             .Select(index => new WeatherForecast
                             {
                                 Date = DateTime.Now.AddDays(index),
                                 TemperatureC = rng.Next(-20, 55),
                                 Summary = Summaries[rng.Next(Summaries.Length)]
                             })
                             .ToArray();
        }
    }
}
