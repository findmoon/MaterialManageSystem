using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.Reels;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    [BindProperty]
    public CreateReelRequest Reel { get; set; } = new CreateReelRequest();

    public List<PartNoDto> PartNos { get; set; } = new();

    private HttpClient CreateHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return httpClient;
    }

    public async Task OnGetAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Response.Redirect("/Login");
            return;
        }

        // 获取物料列表用于下拉选择
        var httpClient = CreateHttpClient();
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

    public async Task<IActionResult> OnPostAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        if (!ModelState.IsValid)
        {
            // 重新加载物料列表
            var httpClient = CreateHttpClient();
            try
            {
                var response = await httpClient.GetFromJsonAsync<PartNoListResponse>("partnos");
                PartNos = response?.Data ?? new List<PartNoDto>();
            }
            catch (Exception)
            {
                PartNos = new List<PartNoDto>();
            }
            return Page();
        }

        var client = CreateHttpClient();
        await client.PostAsJsonAsync("reels", Reel);

        return RedirectToPage("/Reels");
    }
}

public class CreateReelRequest
{
    public string ReelNo { get; set; } = string.Empty;
    public long PartNoId { get; set; }
    public long? CellId { get; set; }
    public decimal InitialQuantity { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class PartNoDto
{
    public long Id { get; set; }
    public string PartNoCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class PartNoListResponse
{
    public List<PartNoDto> Data { get; set; } = new();
}