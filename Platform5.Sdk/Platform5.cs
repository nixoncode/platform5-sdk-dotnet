namespace Platform5.Sdk;

public class Platform5
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public SmsService Sms { get; }
    public EmailService Email { get; }
    public MessagesService Messages { get; }
    public AccountService Account { get; }

    public Platform5(string apiKey, string? baseUrl = null)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _baseUrl = (baseUrl ?? "http://localhost:8084").TrimEnd('/');
        _http = new HttpClient();

        Sms = new SmsService(this);
        Email = new EmailService(this);
        Messages = new MessagesService(this);
        Account = new AccountService(this);
    }

    public async Task HealthAsync()
    {
        await RequestAsync<object>(HttpMethod.Get, "/health");
    }

    internal async Task<T?> RequestAsync<T>(HttpMethod method, string path, object? body = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, _baseUrl + path);
        request.Headers.Add("X-API-Key", _apiKey);

        if (idempotencyKey != null)
            request.Headers.Add("Idempotency-Key", idempotencyKey);

        if (body != null)
            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(body, Json.Options),
                System.Text.Encoding.UTF8,
                "application/json"
            );

        var response = await _http.SendAsync(request);
        var bodyText = await response.Content.ReadAsStringAsync();
        var requestId = response.Headers.TryGetValues("X-Request-ID", out var rid) ? rid.FirstOrDefault() : null;

        if (!response.IsSuccessStatusCode)
        {
            var envelope = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<T>>(bodyText, Json.Options);
            throw ToError((int)response.StatusCode, response.Headers, envelope, requestId);
        }

        var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<T>>(bodyText, Json.Options);
        return result is not null ? result.Data : default;
    }

    private static Platform5Exception ToError(int statusCode, System.Net.Http.Headers.HttpResponseHeaders headers, ApiResponse<object>? envelope, string? requestId)
    {
        var message = envelope?.Message ?? "Unknown error";
        var errors = envelope?.Errors;

        if (statusCode == 429)
        {
            var limit = ParseHeader(headers, "X-RateLimit-Limit");
            var remaining = ParseHeader(headers, "X-RateLimit-Remaining");
            return new RateLimitException(message, errors, requestId, limit, remaining);
        }

        return statusCode switch
        {
            401 => new UnauthorizedException(message, errors, requestId),
            402 => new InsufficientBalanceException(message, errors, requestId),
            403 => new ForbiddenException(message, errors, requestId),
            404 => new NotFoundException(message, errors, requestId),
            422 => new ValidationException(message, errors, requestId),
            _ => new Platform5Exception(statusCode, message, errors, requestId),
        };
    }

    private static int ParseHeader(System.Net.Http.Headers.HttpResponseHeaders headers, string name)
    {
        if (headers.TryGetValues(name, out var values) && int.TryParse(values.FirstOrDefault(), out var result))
            return result;
        return 0;
    }
}

internal static class Json
{
    public static readonly System.Text.Json.JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower,
    };
}
