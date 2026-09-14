using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Application.Interfaces;

namespace BikersStore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
    }

    /// <summary>
    /// Gets all suppliers.
    /// </summary>
    /// <param name="liveErp">If true, queries live supplier objects from Unit4 ERP.</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SupplierListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool liveErp = false, CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierService.GetAllAsync(liveErp, cancellationToken);
        return Ok(suppliers);
    }

    /// <summary>
    /// Searches suppliers by name, ID, email, city, or country.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<SupplierListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierService.SearchAsync(query, cancellationToken);
        return Ok(suppliers);
    }

    /// <summary>
    /// Gets active suppliers lookup for product forms and dropdowns.
    /// </summary>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<SupplierLookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken = default)
    {
        var lookup = await _supplierService.GetLookupAsync(cancellationToken);
        return Ok(lookup);
    }

    /// <summary>
    /// Gets detailed information for a specific supplier.
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="fromErp">If true, queries live master data from Unit4 ERP.</param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplierDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, [FromQuery] bool fromErp = false, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierService.GetByIdAsync(id, fromErp, cancellationToken);
        if (supplier == null)
            return NotFound(new { message = $"Supplier with ID '{id}' was not found." });

        return Ok(supplier);
    }

    /// <summary>
    /// Creates a new supplier in Unit4 ERP test environment and records a local shadow reference copy.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _supplierService.CreateAsync(dto, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.SupplierId }, result.Value);
    }

    /// <summary>
    /// Updates local supplier information.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _supplierService.UpdateAsync(id, dto, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Closes a supplier in Unit4 ERP (sets status to 'C') and marks local record closed.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Close(string id, CancellationToken cancellationToken = default)
    {
        var result = await _supplierService.CloseAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }
}
