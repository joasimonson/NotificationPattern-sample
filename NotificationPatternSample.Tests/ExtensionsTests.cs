using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;

namespace NotificationPatternSample.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void Result_IsSuccess_Returns_SuccessDetails()
    {
        // Arrange
        var successResult = Result.Success();

        // Act
        var result = successResult.Result();

        // Assert
        result.Should().BeOfType<Ok<SuccessDetails>>()
            .Which.Value.Should().BeEquivalentTo(new SuccessDetails
            {
                Status = StatusCodes.Status200OK,
                Title = "Success"
            });
    }

    [Fact]
    public void Result_IsSuccessWithWarnings_Returns_SuccessDetails()
    {
        // Arrange
        var warning = ServiceWarnings.DateLessThanToday;
        var successResult = Result.Success([warning]);

        // Act
        var result = successResult.Result();

        // Assert
        result.Should().BeOfType<Ok<SuccessDetails>>()
            .Which.Value.Should().BeEquivalentTo(new SuccessDetails
            {
                Status = StatusCodes.Status200OK,
                Title = "Success",
                Extensions = new Dictionary<string, object?>
                {
                    { "warnings", new[] { new { warning.Code, warning.Description } } }
                }
            });
    }

    [Fact]
    public void Result_IsFailure_Returns_ProblemDetails()
    {
        // Arrange
        var error = new Notification(NotificationLevel.Error, "123", "Some error");
        var failureResult = Result.Notify(error);

        // Act
        var result = failureResult.Result();

        // Assert
        result.Should().BeOfType<ProblemHttpResult>()
            .Which.ProblemDetails.Should().BeEquivalentTo(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", new[] { new { error.Code, error.Description } } },
                    { "warnings", Array.Empty<object>() }
                }
            });
    }

    [Fact]
    public void ToSuccess_WithErrorResult_ThrowsInvalidOperationException()
    {
        // Arrange
        var failureResult = Result.Notify(new Notification(NotificationLevel.Error, "123", "Some error"));

        // Act
        Action act = () => failureResult.ToSuccess();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Can't convert problem result to success");
    }

    [Fact]
    public void ToProblemDetails_WithSuccessResult_ThrowsInvalidOperationException()
    {
        // Arrange
        var successResult = Result.Success();

        // Act
        Action act = () => successResult.ToProblemDetails();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Can't convert success result to problem");
    }
}