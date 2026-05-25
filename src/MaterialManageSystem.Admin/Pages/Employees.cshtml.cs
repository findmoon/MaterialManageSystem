using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using MaterialManageSystem.Admin.Pages.Employees;

namespace MaterialManageSystem.Admin.Pages;

public class EmployeesModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeesModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<EmployeeDto> Employees { get; set; } = new();

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
}
