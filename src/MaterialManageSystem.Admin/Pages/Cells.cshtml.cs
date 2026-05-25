using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages;

public class CellsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CellsModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CellDto> Cells { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? LayerId { get; set; }

    public async Task OnGetAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        
        if (string.IsNullOrEmpty(token))
        {
            Response.Redirect("/Login");
            return;
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var url = LayerId.HasValue ? $"cells?layerId={LayerId.Value}" : "cells";
            var response = await httpClient.GetFromJsonAsync<CellListResponse>(url);
            Cells = response?.Data ?? new List<CellDto>();
        }
        catch (Exception)
        {
            Cells = new List<CellDto>();
        }
    }
}

public class CellDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public long LayerId { get; set; }
    public string LayerCode { get; set; } = string.Empty;
    public string RackCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CellListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<CellDto>? Data { get; set; }
}