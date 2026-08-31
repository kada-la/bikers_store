using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Domain.Common;
using SalesManagementSystem.Domain.Entities;

namespace SalesManagementSystem.Application.Services;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
}
