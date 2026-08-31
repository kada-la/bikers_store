using System;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;

namespace SalesManagementSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
}
