using System.Threading.Tasks;
using SalesManagementSystem.Application.DTOs;

namespace SalesManagementSystem.Application.Services;

public interface IReportService
{
    Task<DashboardDto> GetDashboardDataAsync();
    Task<ReportsHubDto> GetReportsHubDataAsync();
}
