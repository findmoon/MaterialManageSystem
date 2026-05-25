using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaterialManageSystem.Admin.Pages.Employees;

namespace MaterialManageSystem.Admin.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<EmployeeDto> Employees { get; set; } = new();
    public string? ErrorMessage { get; set; }

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
            var response = await httpClient.GetFromJsonAsync<EmployeeListResponse>("employees");
            Employees = response?.Data ?? new List<EmployeeDto>();
        }
        catch (Exception)
        {
            Employees = new List<EmployeeDto>();
        }
    }

    public async Task<IActionResult> OnPostAsync(string Username, string Password, long EmployeeId, int UserType)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Username,
            Password,
            EmployeeId,
            UserType
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync("users", request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Users");
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                ErrorMessage = result?.Message ?? "创建失败";
                await OnGetAsync();
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await OnGetAsync();
            return Page();
        }
    }
}
