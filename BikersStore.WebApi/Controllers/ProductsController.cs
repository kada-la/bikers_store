using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Application.Interfaces;

namespace BikersStore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    /// <summary>
    /// Retrieves all products with their category and supplier information.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] int? categoryId = null, [FromQuery] bool includeInactive = false)
    {
        if (!string.IsNullOrWhiteSpace(search) || categoryId.HasValue)
        {
            var filtered = await _productService.SearchAndFilterAsync(search, categoryId, includeInactive);
            return Ok(filtered);
        }

        var products = await _productService.GetAllAsync(includeInactive);
        return Ok(products);
    }

    /// <summary>
    /// Gets active products available for sale with pricing, stock, and supplier info.
    /// </summary>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductLookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLookup()
    {
        var lookup = await _productService.GetAvailableForSaleLookupAsync();
        return Ok(lookup);
    }

    /// <summary>
    /// Retrieves detailed product information including sales summary and supplier details.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetDetailsAsync(id);
        if (product == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product with category and optional supplier association.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.CreateAsync(dto);
        if (result.IsFailure)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { productId = result.Value });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductEditDto dto)
    {
        if (id != dto.ProductId)
            return BadRequest(new { message = "Product ID mismatch." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpdateAsync(dto);
        if (result.IsFailure)
            return BadRequest(new { message = result.ErrorMessage });

        return NoContent();
    }

    /// <summary>
    /// Soft deletes a product (sets IsActive = false).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.SoftDeleteAsync(id);
        if (result.IsFailure)
            return BadRequest(new { message = result.ErrorMessage });

        return NoContent();
    }

    /// <summary>
    /// Reactivates an inactive product.
    /// </summary>
    [HttpPost("{id:int}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reactivate(int id)
    {
        var result = await _productService.ReactivateAsync(id);
        if (result.IsFailure)
            return BadRequest(new { message = result.ErrorMessage });

        return NoContent();
    }
}
