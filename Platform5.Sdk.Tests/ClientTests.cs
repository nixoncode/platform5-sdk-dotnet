using System.Net;
using System.Text.Json;
using Platform5.Sdk;
using Xunit;

namespace Platform5.Sdk.Tests;

public class ClientTests
{
    private static Platform5 CreateClient(HttpStatusCode status, object? data, string? errors = null, int? limit = null, int? remaining = null)
    {
        var handler = new MockHttpMessageHandler(status, data, errors, limit, remaining);
        var client = new Platform5("test-key");
        typeof(Platform5).GetField("_http", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(client, new HttpClient(handler));
        return client;
    }

    [Fact]
    public async Task Health_ReturnsSuccessfully()
    {
        var client = CreateClient(HttpStatusCode.OK, null);
        await client.HealthAsync();
    }

    [Fact]
    public async Task SmsSend_ReturnsMessageId()
    {
        var client = CreateClient(HttpStatusCode.OK, new
        {
            message_id = "m1",
            to = "+2547",
            sender_name = "B",
            parts = 1,
            cost = 1.0,
            currency = "KES",
            status = "queued"
        });

        var result = await client.Sms.SendAsync(new SendSmsRequest("+2547", "Hi", "B"));
        Assert.Equal("m1", result.MessageId);
    }

    [Fact]
    public async Task EmailSend_ReturnsMessageId()
    {
        var client = CreateClient(HttpStatusCode.OK, new { message_id = "e1", status = "queued" });
        var result = await client.Email.SendAsync(new SendEmailRequest("a@b.com", "Hi", "Hello", "B"));
        Assert.Equal("e1", result.MessageId);
    }

    [Fact]
    public async Task MessagesGet_ReturnsStatus()
    {
        var client = CreateClient(HttpStatusCode.OK, new
        {
            id = "m1", to = "+2547", sender_name = "B", parts = 1, cost = 1.0,
            status = "delivered", created_at = "2024-01-01T00:00:00Z"
        });
        var result = await client.Messages.GetAsync("m1");
        Assert.Equal("m1", result.Id);
        Assert.Equal("delivered", result.Status);
    }

    [Fact]
    public async Task AccountGetBalance_ReturnsBalance()
    {
        var client = CreateClient(HttpStatusCode.OK, new { available_balance = 1250.50, current_balance = 1500.00, currency = "KES" });
        var result = await client.Account.GetBalanceAsync();
        Assert.Equal(1250.50, result.AvailableBalance);
    }

    [Fact]
    public async Task Unauthorized_Throws()
    {
        var client = CreateClient(HttpStatusCode.Unauthorized, null, "bad key");
        await Assert.ThrowsAsync<UnauthorizedException>(() => client.HealthAsync());
    }

    [Fact]
    public async Task RateLimit_IncludesHeaders()
    {
        var client = CreateClient((HttpStatusCode)429, null, "too fast", limit: 50, remaining: 3);
        var ex = await Assert.ThrowsAsync<RateLimitException>(() => client.HealthAsync());
        Assert.Equal(50, ex.Limit);
        Assert.Equal(3, ex.Remaining);
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _status;
    private readonly object? _data;
    private readonly string? _errors;
    private readonly int? _limit;
    private readonly int? _remaining;

    public MockHttpMessageHandler(HttpStatusCode status, object? data, string? errors = null, int? limit = null, int? remaining = null)
    {
        _status = status;
        _data = data;
        _errors = errors;
        _limit = limit;
        _remaining = remaining;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_status);
        response.Headers.Add("X-Request-ID", "req-123");
        if (_limit.HasValue) response.Headers.Add("X-RateLimit-Limit", _limit.Value.ToString());
        if (_remaining.HasValue) response.Headers.Add("X-RateLimit-Remaining", _remaining.Value.ToString());

        var envelope = new { success = (int)_status < 400, message = "", data = _data, errors = _errors };
        response.Content = new StringContent(JsonSerializer.Serialize(envelope), System.Text.Encoding.UTF8, "application/json");

        return Task.FromResult(response);
    }
}
