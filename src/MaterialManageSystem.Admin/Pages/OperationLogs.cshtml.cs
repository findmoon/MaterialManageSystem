using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaterialManageSystem.Admin.Pages;

public class OperationLogsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OperationLogsModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<OperationLogDto> Logs { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public new int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 20;

    public int Total { get; set; }

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
            var url = $"operationlogs?page={Page}&pageSize={PageSize}";
            if (!string.IsNullOrEmpty(Keyword))
            {
                url += $"&keyword={Keyword}";
            }
            var response = await httpClient.GetFromJsonAsync<OperationLogListResponse>(url);
            Logs = response?.Data ?? new List<OperationLogDto>();
            Total = response?.Total ?? 0;
        }
        catch (Exception)
        {
            Logs = new List<OperationLogDto>();
        }
    }
}

public class OperationLogDto
{
    public long Id { get; set; }
    public string? OperationType { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public long? ExecutionTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OperationLogListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<OperationLogDto>? Data { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
