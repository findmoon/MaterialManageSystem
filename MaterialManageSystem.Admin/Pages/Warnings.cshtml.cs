using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaterialManageSystem.Admin.Pages;

public class WarningsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WarningsModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<WarningDto> Warnings { get; set; } = new();

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
            var response = await httpClient.GetFromJsonAsync<WarningSummaryResponse>("dashboard/warning-summary");
            Warnings = response?.Data?.Warnings ?? new List<WarningDto>();
        }
        catch (Exception)
        {
            Warnings = new List<WarningDto>();
        }
    }
}

public class WarningDto
{
    public long Id { get; set; }
    public long ReelId { get; set; }
    public string ReelNo { get; set; } = string.Empty;
    public string PartNoName { get; set; } = string.Empty;
    public int WarningType { get; set; }
    public int WarningLevel { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WarningSummaryResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public WarningSummaryData? Data { get; set; }
}

public class WarningSummaryData
{
    public int TotalWarnings { get; set; }
    public int CriticalWarnings { get; set; }
    public int NormalWarnings { get; set; }
    public List<WarningDto> Warnings { get; set; } = new();
}