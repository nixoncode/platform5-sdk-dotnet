using System.Text.Json.Serialization;

namespace Platform5.Sdk;

public record SendSmsRequest(
    [property: JsonPropertyName("to")] string To,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("from")] string From
);

public record SendSmsResponse(
    [property: JsonPropertyName("message_id")] string MessageId,
    [property: JsonPropertyName("to")] string To,
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("parts")] int Parts,
    [property: JsonPropertyName("cost")] double Cost,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("status")] string Status
);

public record SendEmailRequest(
    [property: JsonPropertyName("to")] string To,
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("from")] string From,
    [property: JsonPropertyName("body_type")] string? BodyType = null
);

public record SendEmailResponse(
    [property: JsonPropertyName("message_id")] string MessageId,
    [property: JsonPropertyName("status")] string Status
);

public record MessageStatusResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("to")] string To,
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("parts")] int Parts,
    [property: JsonPropertyName("cost")] double Cost,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] string CreatedAt,
    [property: JsonPropertyName("sent_at")] string? SentAt = null,
    [property: JsonPropertyName("delivered_at")] string? DeliveredAt = null,
    [property: JsonPropertyName("error")] string? Error = null
);

public record BalanceResponse(
    [property: JsonPropertyName("available_balance")] double AvailableBalance,
    [property: JsonPropertyName("current_balance")] double CurrentBalance,
    [property: JsonPropertyName("currency")] string Currency
);

internal record ApiResponse<T>(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("data")] T? Data,
    [property: JsonPropertyName("errors")] string? Errors
);
