using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace MaterialManageSystem.Admin.Pages;

public class LoginModel : PageModel
{
    private readonly HttpClient _httpClient;

    public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        var apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl is not configured");
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new { Username, Password });
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                
                if (loginResponse != null && loginResponse.Data != null)
                {
                    HttpContext.Session.SetString("Token", loginResponse.Data.Token);
                    return RedirectToPage("/Dashboard");
                }
            }
            
            ErrorMessage = "用户名或密码错误";
        }
        catch (Exception)
        {
            ErrorMessage = "登录失败，请稍后重试";
        }

        return Page();
    }
}

public class LoginResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public LoginData? Data { get; set; }
}

public class LoginData
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}