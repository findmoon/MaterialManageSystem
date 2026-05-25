using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaterialManageSystem.Admin.Pages;

public class DashboardModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public DashboardDto? Dashboard { get; set; }

    public decimal InStockPercent => Dashboard != null && Dashboard.TotalReels > 0 
        ? (decimal)Dashboard.InStockReels / Dashboard.TotalReels * 100 : 0;

    public decimal OutStockPercent => Dashboard != null && Dashboard.TotalReels > 0 
        ? (decimal)Dashboard.OutStockReels / Dashboard.TotalReels * 100 : 0;

    public decimal OnlinePercent => Dashboard != null && Dashboard.TotalReels > 0 
        ? (decimal)Dashboard.OnlineReels / Dashboard.TotalReels * 100 : 0;

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
            var response = await httpClient.GetFromJsonAsync<ApiResponse<DashboardDto>>("dashboard/overview");
            Dashboard = response?.Data;
        }
        catch (Exception)
        {
            Dashboard = null;
        }
    }
}

public class DashboardDto
{
    public int TotalWarehouses { get; set; }
    public int TotalPartNos { get; set; }
    public int TotalReels { get; set; }
    public int InStockReels { get; set; }
    public int OutStockReels { get; set; }
    public int OnlineReels { get; set; }
    public int ActiveWarnings { get; set; }
}

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
