using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using MaterialManageSystem.Admin.Pages.Users;

namespace MaterialManageSystem.Admin.Pages;

public class UsersModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsersModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<UserDto> Users { get; set; } = new();

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
            var response = await httpClient.GetFromJsonAsync<UserListResponse>("users");
            Users = response?.Data ?? new List<UserDto>();
        }
        catch (Exception)
        {
            Users = new List<UserDto>();
        }
    }
}
