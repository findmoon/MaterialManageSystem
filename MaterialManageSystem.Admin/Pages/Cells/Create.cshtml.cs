using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages.Cells;

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
    public CreateCellRequest Input { get; set; } = new CreateCellRequest();

    public List<RackDto> Racks { get; set; } = new();
    public List<LayerDto> Layers { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? SelectedRackId { get; set; }

    public async Task OnGetAsync()
    {
        await LoadRacks();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRacks();
            return Page();
        }

        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return Redirect("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await httpClient.PostAsJsonAsync("cells", Input);

        return RedirectToPage("/Cells");
    }

    public async Task<JsonResult> OnGetLayersAsync(long rackId)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return new JsonResult(new List<LayerDto>());
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetFromJsonAsync<LayerListResponse>($"cells/layers?rackId={rackId}");
        return new JsonResult(response?.Data ?? new List<LayerDto>());
    }

    private async Task LoadRacks()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return;

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetFromJsonAsync<RackListResponse>("cells/racks");
        Racks = response?.Data ?? new List<RackDto>();
    }
}

public class CreateCellRequest
{
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public long LayerId { get; set; }
}

public class RackDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
}

public class LayerDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class RackListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<RackDto>? Data { get; set; }
}

public class LayerListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<LayerDto>? Data { get; set; }
}