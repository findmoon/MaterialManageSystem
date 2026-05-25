using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace MaterialManageSystem.Admin.Pages;

public class RolesModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RolesModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<RoleDto> Roles { get; set; } = new();

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
            var response = await httpClient.GetFromJsonAsync<RoleListResponse>("roles");
            Roles = response?.Data ?? new List<RoleDto>();
        }
        catch (Exception)
        {
            Roles = new List<RoleDto>();
        }
    }
}

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class RoleListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<RoleDto>? Data { get; set; }
}