using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaterialManageSystem.Admin.Pages;

public class PartNosModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PartNosModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<PartNoDto> PartNos { get; set; } = new();

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
            var response = await httpClient.GetFromJsonAsync<PartNoListResponse>("partnos");
            PartNos = response?.Data ?? new List<PartNoDto>();
        }
        catch (Exception)
        {
            PartNos = new List<PartNoDto>();
        }
    }
}

public class PartNoDto
{
    public long Id { get; set; }
    public string PartNoCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Size { get; set; }
    public string? Packaging { get; set; }
    public string Unit { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal? WarningQuantity { get; set; }
    public int? WarningDays { get; set; }
}

public class PartNoListResponse
{
    public List<PartNoDto> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
