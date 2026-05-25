using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaterialManageSystem.Admin.Pages.Employees;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public EmployeeDto? Employee { get; set; }
    public List<UserDto> AvailableUsers { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(long id)
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
            var employeeResponse = await httpClient.GetFromJsonAsync<EmployeeResponse>($"employees/{id}");
            Employee = employeeResponse?.Data;

            var usersResponse = await httpClient.GetFromJsonAsync<UserListResponse>("users");
            AvailableUsers = usersResponse?.Data ?? new List<UserDto>();
        }
        catch (Exception)
        {
            Employee = null;
        }
    }

    public async Task<IActionResult> OnPostAsync(long id, string EmployeeNo, string Name, string? Department, string? Position, string? Phone, string? Email, long? UserId, bool IsActive)
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
            EmployeeNo,
            Name,
            Department,
            Position,
            Phone,
            Email,
            UserId = UserId ?? 0,
            IsActive
        };

        try
        {
            var response = await httpClient.PutAsJsonAsync($"employees/{id}", request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees");
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                ErrorMessage = result?.Message ?? "更新失败";
                await OnGetAsync(id);
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await OnGetAsync(id);
            return Page();
        }
    }
}
