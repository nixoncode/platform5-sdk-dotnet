# Platform5.Sdk

.NET SDK for the Platform5 Developer API.

## Install

```sh
dotnet add package Platform5.Sdk
```

## Usage

```csharp
using Platform5.Sdk;

var client = new Platform5("p5_live_abc123def456");

// Send an SMS
var sms = await client.Sms.SendAsync(new SendSmsRequest(
    To: "+254712345678",
    Message: "Your appointment is confirmed.",
    From: "MyBrand"
));
Console.WriteLine(sms.MessageId);

// Send an email
await client.Email.SendAsync(new SendEmailRequest(
    To: "user@example.com",
    Subject: "Welcome",
    Body: "Hello!",
    From: "MyBrand"
));

// Check message status
var status = await client.Messages.GetAsync("msg-uuid");
Console.WriteLine(status.Status);

// Check balance
var balance = await client.Account.GetBalanceAsync();
Console.WriteLine($"{balance.AvailableBalance} {balance.Currency}");
```

## Services

| Method | Endpoint |
|--------|----------|
| `client.Sms.SendAsync(request)` | POST /v1/sms/send |
| `client.Email.SendAsync(request)` | POST /v1/email/send |
| `client.Messages.GetAsync(id)` | GET /v1/messages/{id} |
| `client.Account.GetBalanceAsync()` | GET /v1/balance |
| `client.HealthAsync()` | GET /health |

## Error Handling

```csharp
try
{
    await client.Sms.SendAsync(request);
}
catch (RateLimitException e)
{
    Console.WriteLine($"Rate limited: {e.Remaining}/{e.Limit}");
}
catch (Platform5Exception e)
{
    Console.WriteLine($"API error {e.StatusCode}: {e.Message}");
}
```

## Requirements

- .NET 8.0+
