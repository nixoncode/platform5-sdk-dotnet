namespace Platform5.Sdk;

public class MessagesService
{
    private readonly Platform5 _client;

    internal MessagesService(Platform5 client) => _client = client;

    public async Task<MessageStatusResponse> GetAsync(string id)
    {
        return (await _client.RequestAsync<MessageStatusResponse>(HttpMethod.Get, $"/v1/messages/{id}"))!;
    }
}
