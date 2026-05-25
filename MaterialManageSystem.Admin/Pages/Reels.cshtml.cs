using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages;

public class ReelsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReelsModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<ReelDto> Reels { get; set; } = new();

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
            var response = await httpClient.GetFromJsonAsync<ReelListResponse>("reels");
            Reels = response?.Data ?? new List<ReelDto>();
        }
        catch (Exception)
        {
            Reels = new List<ReelDto>();
        }
    }

    public async Task<IActionResult> OnGetActionAsync(long id, string action)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var request = new UsageUploadRequest
            {
                ReelId = id,
                EmployeeId = 1,
                UsageType = GetUsageType(action),
                Quantity = 0
            };

            await httpClient.PutAsJsonAsync($"reels/{id}/{action}", request);
        }
        catch (Exception)
        {
            // 忽略错误
        }

        return RedirectToPage("/Reels");
    }

    private int GetUsageType(string action)
    {
        return action.ToLower() switch
        {
            "checkout" => 1,
            "online" => 2,
            "return" => 3,
            "scrap" => 4,
            _ => 1
        };
    }
}

public class ReelDto
{
    public long Id { get; set; }
    public string ReelNo { get; set; } = string.Empty;
    public long PartNoId { get; set; }
    public string PartNoCode { get; set; } = string.Empty;
    public string PartNoName { get; set; } = string.Empty;
    public long? CellId { get; set; }
    public string? CellLocation { get; set; }
    public decimal InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class ReelListResponse
{
    public List<ReelDto> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UsageUploadRequest
{
    public long ReelId { get; set; }
    public long EmployeeId { get; set; }
    public int UsageType { get; set; }
    public decimal Quantity { get; set; }
}