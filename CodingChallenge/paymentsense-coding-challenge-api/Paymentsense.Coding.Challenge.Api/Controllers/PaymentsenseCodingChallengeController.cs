using Microsoft.AspNetCore.Mvc;

namespace Paymentsense.Coding.Challenge.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsenseCodingChallengeController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Paymentsense Coding Challenge!");
        }
    }
}
