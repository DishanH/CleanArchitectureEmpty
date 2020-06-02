using CleanArchitectureEmpty.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureEmpty.WebUI.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/WeatherForecast")]
    //[Route("api/v{version:apiVersion}/WeatherForecast")] /*Part of a URI*/
    public class WeatherForecastController : ApiController
    {

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return (await Mediator.Send(new GetWeatherForecastsQuery())).Take(1);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IEnumerable<WeatherForecast>> GetV2()
        {
            return (await Mediator.Send(new GetWeatherForecastsQuery())).Take(2);
        }
    }
    [ApiVersion("3.0")]
    [Route("api/WeatherForecast")]
    public class WeatherForecastV2Controller : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return (await Mediator.Send(new GetWeatherForecastsQuery())).Take(3);
        }
    }
}
