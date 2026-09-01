using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Application.Interfaces;

namespace BikersStore.Controllers;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _saleService.GetAllAsync();
        return View(sales);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await _saleService.GetDetailsAsync(id);
        if (model == null)
        {
            TempData["Error"] = "Sale transaction not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = await _saleService.PrepareCreateViewModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCreateDto model)
    {
        if (model.Items == null || !model.Items.Any(i => i.ProductId > 0 && i.Quantity > 0))
        {
            ModelState.AddModelError(string.Empty, "At least one product item with valid quantity is required.");
        }

        if (!ModelState.IsValid)
        {
            var freshModel = await _saleService.PrepareCreateViewModelAsync();
            model.AvailableCustomers = freshModel.AvailableCustomers;
            model.AvailableProducts = freshModel.AvailableProducts;
            return View(model);
        }

        var result = await _saleService.CreateSaleAsync(model);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            var freshModel = await _saleService.PrepareCreateViewModelAsync();
            model.AvailableCustomers = freshModel.AvailableCustomers;
            model.AvailableProducts = freshModel.AvailableProducts;
            return View(model);
        }

        TempData["Success"] = $"Sale #{result.Value} created successfully!";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _saleService.GetDetailsAsync(id);
        if (model == null)
        {
            TempData["Error"] = "Sale not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _saleService.DeleteSaleAsync(id);
        if (result.IsFailure)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["Success"] = "Sale record deleted and product inventory restored successfully.";
        return RedirectToAction(nameof(Index));
    }
}
