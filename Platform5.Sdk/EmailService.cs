namespace Platform5.Sdk;

public class EmailService
{
    private readonly Platform5 _client;

    internal EmailService(Platform5 client) => _client = client;

    public async Task<SendEmailResponse> SendAsync(SendEmailRequest request)
    {
        var key = Guid.NewGuid().ToString();
        return (await _client.RequestAsync<SendEmailResponse>(HttpMethod.Post, "/v1/email/send", request, key))!;
    }
}
