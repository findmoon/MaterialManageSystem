using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.PartNos;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public PartNoDto PartNo { get; set; } = new PartNoDto();

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetFromJsonAsync<PartNoResponse>($"partnos/{id}");
        if (response?.Data == null)
        {
            return RedirectToPage("/PartNos");
        }

        PartNo = response.Data;
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

        await httpClient.DeleteAsync($"partnos/{id}");

        return RedirectToPage("/PartNos");
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

public class PartNoResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public PartNoDto? Data { get; set; }
}