using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Controllers;

[Route("[Controller]")]
public class StatusController : Controller
{
    [HttpGet]
    public IActionResult Get() => new ContentResult { StatusCode = 200, Content = "Online" };
}