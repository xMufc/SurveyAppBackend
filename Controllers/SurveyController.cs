using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppBackend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        [HttpPost("create_survey")]
        public async Task<IActionResult> CreateSurvey([FromBody] Register model)
        {
            // Implementation for creating a survey
            return Ok();
        }
    }
}
