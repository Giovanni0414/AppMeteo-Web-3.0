using AppMeteo_3._0.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppMeteo_3._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiMeteoController : ControllerBase
    {
        private readonly ApiServices _apiServices;

        public ApiMeteoController(ApiServices apiServices)
        {
            _apiServices = apiServices ?? throw new ArgumentNullException(nameof(apiServices));
        }

        [HttpGet("get-meteo")]
        public async Task<IActionResult> GetMeteo(string cittaDaCercare)
        {
            try
            {
                var meteoData = await _apiServices.GetMeteo(cittaDaCercare);
                return Ok(meteoData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
