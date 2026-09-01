using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Application.Services;

namespace BikersStore.Controllers;

public class DashboardController : Controller
{
    private readonly IReportService _reportService;

    public DashboardController(IReportService reportService)
    {
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _reportService.GetDashboardDataAsync();
        return View(dashboardData);
    }
}
