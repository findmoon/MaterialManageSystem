using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.Cells;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public CellDto Cell { get; set; } = new CellDto();

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetFromJsonAsync<CellResponse>($"cells/{id}");
        if (response?.Data == null)
        {
            return RedirectToPage("/Cells");
        }

        Cell = response.Data;
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

        await httpClient.DeleteAsync($"cells/{id}");

        return RedirectToPage("/Cells");
    }
}