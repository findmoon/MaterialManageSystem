using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MaterialManageSystem.Admin.Pages;

namespace MaterialManageSystem.Admin.Pages.Warehouses;

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
    public WarehouseDto Warehouse { get; set; } = new WarehouseDto();

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

    public async Task OnGetAsync(int? id)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Response.Redirect("/Login");
            return;
        }

        if (id.HasValue)
        {
            var httpClient = CreateHttpClient();
            var response = await httpClient.GetFromJsonAsync<ApiResponse<WarehouseDto>>($"warehouses/{id}");
            Warehouse = response?.Data ?? new WarehouseDto();
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
            return Page();
        }

        var httpClient = CreateHttpClient();

        if (Warehouse.Id > 0)
        {
            await httpClient.PutAsJsonAsync($"warehouses/{Warehouse.Id}", Warehouse);
        }
        else
        {
            await httpClient.PostAsJsonAsync("warehouses", Warehouse);
        }

        return RedirectToPage("/Warehouses");
    }
}
