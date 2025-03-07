using api_gateway.Core.Application.DTOs;
using api_gateway.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace api_gateway.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly IStaticDataService _staticDataService;
        public SettingController(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
        [HttpPost("set-claims")]
        public async Task<IActionResult> SetClaims([FromBody] List<Claim> claims)
        {
            try
            {

                string job1 = await _staticDataService.InitializeStaticDataService();
                string job2 = await _staticDataService.GetAllTheClaims();
                if (job1 == "200" && job2 == "200")
                    return Ok(new
                    {
                        status = 500,
                        message = "success",
                        data = "",
                        errors = new List<string> {  }
                    });
                else {
                    return StatusCode(StatusCodes.Status500InternalServerError,new
                    {
                        status = 500,
                        message = "internal server error",
                        data = "",
                        errors = new List<string> { job2, job1 }
                    }
                    );
                } 
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
