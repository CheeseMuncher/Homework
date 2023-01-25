using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Controllers;

[Route("[Controller]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => new ContentResult { StatusCode = 200, Content = "Online" };
}