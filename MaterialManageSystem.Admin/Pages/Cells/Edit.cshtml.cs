using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.Cells;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    [BindProperty]
    public UpdateCellRequest Input { get; set; } = new UpdateCellRequest();

    public long CellId { get; set; }

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

        CellId = id;
        Input.Code = response.Data.Code;
        Input.Row = response.Data.Row;
        Input.Col = response.Data.Col;
        Input.IsActive = response.Data.IsActive;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long id)
    {
        if (!ModelState.IsValid)
        {
            CellId = id;
            return Page();
        }

        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await httpClient.PutAsJsonAsync($"cells/{id}", Input);

        return RedirectToPage("/Cells");
    }
}

public class UpdateCellRequest
{
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsActive { get; set; }
}

public class CellResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public CellDto? Data { get; set; }
}