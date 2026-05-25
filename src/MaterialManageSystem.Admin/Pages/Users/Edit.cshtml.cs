using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaterialManageSystem.Admin.Pages.Users;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public new UserDto? User { get; set; }
    public List<EmployeeDto> AvailableEmployees { get; set; } = new();
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
            var response = await httpClient.GetFromJsonAsync<UserResponse>($"users/{id}");
            User = response?.Data;

            var employeesResponse = await httpClient.GetFromJsonAsync<EmployeeListResponse>("employees");
            AvailableEmployees = employeesResponse?.Data ?? new List<EmployeeDto>();
        }
        catch (Exception)
        {
            User = null;
        }
    }

    public async Task<IActionResult> OnPostAsync(long id, string? Password, long? EmployeeId, int UserType, bool IsActive)
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
            Password,
            EmployeeId = EmployeeId ?? 0,
            UserType,
            IsActive
        };

        try
        {
            var response = await httpClient.PutAsJsonAsync($"users/{id}", request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Users");
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
