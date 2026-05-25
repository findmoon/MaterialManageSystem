using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaterialManageSystem.Admin.Pages.Employees;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string EmployeeNo, string Name, string? Department, string? Position, string? Phone, string? Email)
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
            Email
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync("employees", request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees");
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                ErrorMessage = result?.Message ?? "创建失败";
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
