using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SalesManagementSystem.Application.Common;
using SalesManagementSystem.Application.DTOs;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Domain.Common;

namespace SalesManagementSystem.Infrastructure.ExternalServices;

public class Unit4SupplierClient : IUnit4SupplierClient
{
    private readonly HttpClient _httpClient;
    private readonly Unit4Settings _settings;
    private readonly ILogger<Unit4SupplierClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public Unit4SupplierClient(
        HttpClient httpClient,
        IOptions<Unit4Settings> settings,
        ILogger<Unit4SupplierClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
            var baseUri = _settings.BaseUrl.EndsWith("/") ? _settings.BaseUrl : _settings.BaseUrl + "/";
            _httpClient.BaseAddress = new Uri(baseUri);
        }

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(_settings.Username) && !string.IsNullOrWhiteSpace(_settings.Password))
        {
            var authString = $"{_settings.Username}:{_settings.Password}";
            var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authBase64);
        }

        if (!string.IsNullOrWhiteSpace(_settings.Client))
        {
            _httpClient.DefaultRequestHeaders.Remove("client");
            _httpClient.DefaultRequestHeaders.Add("client", _settings.Client);
            _httpClient.DefaultRequestHeaders.Remove("U4-Client");
            _httpClient.DefaultRequestHeaders.Add("U4-Client", _settings.Client);
        }
    }

    private string GetCompanyId(string? overrideCompanyId = null)
    {
        return !string.IsNullOrWhiteSpace(overrideCompanyId) ? overrideCompanyId : _settings.Client;
    }

    public async Task<Result<Unit4SupplierDto>> CreateSupplierAsync(Unit4SupplierDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var companyId = GetCompanyId(dto.CompanyId);
            if (string.IsNullOrWhiteSpace(dto.CompanyId))
            {
                dto.CompanyId = companyId;
            }

            var requestUrl = string.IsNullOrWhiteSpace(companyId)
                ? "v1/suppliers"
                : $"v1/suppliers?companyId={Uri.EscapeDataString(companyId)}";

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(dto, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            _logger.LogInformation("Sending POST request to Unit4 ERP: {Url} for supplier '{SupplierId}'", requestUrl, dto.SupplierId);
            var response = await _httpClient.PostAsync(requestUrl, jsonContent, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var created = JsonSerializer.Deserialize<Unit4SupplierDto>(responseBody, _jsonOptions) ?? dto;
                return Result.Success(created);
            }

            _logger.LogWarning("Unit4 ERP CreateSupplier failed with status {StatusCode}: {ResponseBody}", response.StatusCode, responseBody);
            return Result.Failure<Unit4SupplierDto>($"Unit4 ERP returned {(int)response.StatusCode} ({response.StatusCode}): {responseBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while calling Unit4 ERP CreateSupplier for '{SupplierId}'", dto?.SupplierId);
            return Result.Failure<Unit4SupplierDto>($"Failed to connect to Unit4 ERP: {ex.Message}");
        }
    }

    public async Task<Result<Unit4SupplierDto>> GetSupplierAsync(string supplierId, string? companyId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = GetCompanyId(companyId);
            var requestUrl = string.IsNullOrWhiteSpace(company)
                ? $"v1/suppliers/{Uri.EscapeDataString(supplierId)}"
                : $"v1/suppliers/{Uri.EscapeDataString(supplierId)}?companyId={Uri.EscapeDataString(company)}";

            _logger.LogInformation("Sending GET request to Unit4 ERP: {Url}", requestUrl);
            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var supplier = JsonSerializer.Deserialize<Unit4SupplierDto>(responseBody, _jsonOptions);
                return supplier != null
                    ? Result.Success(supplier)
                    : Result.Failure<Unit4SupplierDto>("Failed to deserialize Unit4 ERP supplier response.");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result.Failure<Unit4SupplierDto>($"Supplier '{supplierId}' not found in Unit4 ERP.");
            }

            return Result.Failure<Unit4SupplierDto>($"Unit4 ERP error {(int)response.StatusCode}: {responseBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while calling Unit4 ERP GetSupplier for '{SupplierId}'", supplierId);
            return Result.Failure<Unit4SupplierDto>($"Failed to connect to Unit4 ERP: {ex.Message}");
        }
    }

    public async Task<Result> CloseSupplierAsync(string supplierId, string? companyId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = GetCompanyId(companyId);
            var requestUrl = string.IsNullOrWhiteSpace(company)
                ? $"v1/suppliers/{Uri.EscapeDataString(supplierId)}"
                : $"v1/suppliers/{Uri.EscapeDataString(supplierId)}?companyId={Uri.EscapeDataString(company)}";

            _logger.LogInformation("Sending DELETE (Close) request to Unit4 ERP: {Url}", requestUrl);
            var response = await _httpClient.DeleteAsync(requestUrl, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            return Result.Failure($"Unit4 ERP close failed with status {(int)response.StatusCode}: {responseBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while calling Unit4 ERP CloseSupplier for '{SupplierId}'", supplierId);
            return Result.Failure($"Failed to connect to Unit4 ERP: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<Unit4SupplierDto>>> SearchSuppliersAsync(string? filter = null, int offset = 0, int limit = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = GetCompanyId();
            var queryParams = new List<string>
            {
                $"offset={offset}",
                $"limit={limit}"
            };

            if (!string.IsNullOrWhiteSpace(company))
            {
                queryParams.Add($"companyId={Uri.EscapeDataString(company)}");
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
            }

            var requestUrl = $"v1/objects/suppliers?{string.Join("&", queryParams)}";
            _logger.LogInformation("Sending GET request to Unit4 ERP objects: {Url}", requestUrl);

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var list = JsonSerializer.Deserialize<List<Unit4SupplierDto>>(responseBody, _jsonOptions) ?? new List<Unit4SupplierDto>();
                return Result.Success<IReadOnlyList<Unit4SupplierDto>>(list);
            }

            return Result.Failure<IReadOnlyList<Unit4SupplierDto>>($"Unit4 ERP search error {(int)response.StatusCode}: {responseBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while calling Unit4 ERP SearchSuppliers");
            return Result.Failure<IReadOnlyList<Unit4SupplierDto>>($"Failed to connect to Unit4 ERP: {ex.Message}");
        }
    }
}
