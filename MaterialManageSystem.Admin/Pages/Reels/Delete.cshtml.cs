using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.Reels;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public ReelDto Reel { get; set; } = new ReelDto();

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetFromJsonAsync<ReelResponse>($"reels/{id}");
        if (response?.Data == null)
        {
            return RedirectToPage("/Reels");
        }

        Reel = response.Data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long id)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await httpClient.DeleteAsync($"reels/{id}");

        return RedirectToPage("/Reels");
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

public class ReelResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public ReelDto? Data { get; set; }
}