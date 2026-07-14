namespace Platform5.Sdk;

public class SmsService
{
    private readonly Platform5 _client;

    internal SmsService(Platform5 client) => _client = client;

    public async Task<SendSmsResponse> SendAsync(SendSmsRequest request)
    {
        var key = Guid.NewGuid().ToString();
        return (await _client.RequestAsync<SendSmsResponse>(HttpMethod.Post, "/v1/sms/send", request, key))!;
    }
}
