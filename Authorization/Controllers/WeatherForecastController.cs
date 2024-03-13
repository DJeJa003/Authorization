using Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
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
        private readonly IConfiguration _configuration;
        
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("AppSettingsTest")]
        public IActionResult GetSimple()
        {
            var simppeli = _configuration["SimpleValue"];
            var monimutkainen = _configuration["AppSettings:ApiKey"];

            var result = new
            {
                SimpleValue = simppeli,
                ApiKey = monimutkainen
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("AuthGet")]
        public IEnumerable<WeatherForecast> Get2()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("OpenGet")]
        public IEnumerable <WeatherForecast> Get3() 
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            if (credentials.Username == "testuser" && credentials.Password == "testpassword")
            {
                var tokenService = new TokenService(_configuration);
                var token = tokenService.GenerateToken(credentials.Username, false);
                return Ok(new { Token = token });
            }
            else if (credentials.Username == "admin" && credentials.Password == "adminpassword")
            {
                var tokenService = new TokenService(_configuration);
                var token = tokenService.GenerateToken(credentials.Username, true);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Käyttäjätunnus tai salasana on väärin.");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetSecret")]
        public IActionResult GetSecretData()
        {
            Console.WriteLine($"User {User.Identity.Name} claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            return Ok("Tämä on suojattua tietoa vain Admin-roolia käyttäviltä.");
        }
    }
}
