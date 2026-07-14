namespace Platform5.Sdk;

public class AccountService
{
    private readonly Platform5 _client;

    internal AccountService(Platform5 client) => _client = client;

    public async Task<BalanceResponse> GetBalanceAsync()
    {
        return (await _client.RequestAsync<BalanceResponse>(HttpMethod.Get, "/v1/balance"))!;
    }
}
