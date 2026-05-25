using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaterialManageSystem.Admin.Pages;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string Username, string Password, string ConfirmPassword)
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "用户名和密码不能为空";
            return Page();
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "密码长度至少为6位";
            return Page();
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "两次输入的密码不一致";
            return Page();
        }

        var httpClient = _httpClientFactory.CreateClient("ApiClient");

        var request = new
        {
            Username,
            Password,
            EmployeeId = 0
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync("auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "注册成功！正在跳转登录页面...";
                Response.Headers["Refresh"] = "2;url=/Login";
                return Page();
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                ErrorMessage = result?.Message ?? "注册失败";
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

public class ApiErrorResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
}
