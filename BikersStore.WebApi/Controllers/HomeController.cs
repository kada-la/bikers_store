using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BikersStore.WebApi.Controllers;

public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("api/home")]
    public IActionResult Index()
    {
        return Ok(new { message = "Welcome to BikersStore API!" });
    }
}