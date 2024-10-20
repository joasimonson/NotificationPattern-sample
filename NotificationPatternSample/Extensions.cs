using System.Text.Json.Serialization;

public static class ResultExtensions
{
    public static IResult Result(this Result result)
        => result.IsSuccess ? result.ToSuccess() : result.ToProblemDetails();

    public static IResult ToSuccess(this Result result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Can't convert problem result to success");

        var details = new SuccessDetails()
        {
            Status = StatusCodes.Status200OK,
            Title = "Success"
        };

        if (result.Warnings.Count != 0)
        {
            var warnings = result.Warnings.Select(w => new { w.Code, w.Description });

            details.Extensions = new Dictionary<string, object?>
            {
                { "warnings", warnings }
            };
        }

        return Results.Ok(details);
    }

    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Can't convert success result to problem");

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            extensions: new Dictionary<string, object?>
            {
                { "errors", result.Errors.Select(e => new { e.Code, e.Description }) },
                { "warnings", result.Warnings.Select(w => new { w.Code, w.Description }) }
            });
    }
}

public class SuccessDetails
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-4)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-3)]
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-2)]
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.Ordinal);
}
