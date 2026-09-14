using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Application.Interfaces;

namespace BikersStore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IReportService _reportService;

    public DashboardController(IReportService reportService)
    {
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboardData = await _reportService.GetDashboardDataAsync();
        return Ok(dashboardData);
    }
}
