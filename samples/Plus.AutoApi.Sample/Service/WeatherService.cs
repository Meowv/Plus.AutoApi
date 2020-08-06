using Microsoft.AspNetCore.Mvc;
using Plus.AutoApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plus.AutoApi.Sample.Service
{
    [AutoApi]
    public class WeatherService : IAutoApi
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IEnumerable<WeatherForecast> Get()
        {
            return WeatherForecast();
        }

        [HttpGet("{id}")]
        public IEnumerable<WeatherForecast> Get(int id)
        {
            return WeatherForecast();
        }

        public IEnumerable<WeatherForecast> Post()
        {
            return WeatherForecast();
        }

        [HttpPut("{id}")]
        public IEnumerable<WeatherForecast> Put(int id)
        {
            return WeatherForecast();
        }

        [HttpDelete("{id}")]
        public IEnumerable<WeatherForecast> Delete(int id)
        {
            return WeatherForecast();
        }

        private static IEnumerable<WeatherForecast> WeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();
        }
    }
}