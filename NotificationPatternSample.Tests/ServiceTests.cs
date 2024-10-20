using FluentAssertions;
using Xunit;

namespace NotificationPatternSample.Tests;

public class ServiceTests
{
    private readonly Service _sut = new();
    private readonly DateTime _validDate = DateTime.Now.Date;
    private const bool _valid = true;
    private const bool _invalid = false;

    [Fact]
    public async Task Operation_DateGreaterThanToday_ReturnsError()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var failureExpected = Result.Notify(new Notification(NotificationLevel.Error, "Service.GreaterThanToday", "Operation date greater than today"));

        // Act
        var result = await _sut.Operation(futureDate, _valid, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(failureExpected);
    }

    [Fact]
    public async Task Operation_DateLessThanToday_ReturnsWarning()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(-1);
        var failureExpected = Result.Notify(new Notification(NotificationLevel.Warning, "Service.DateLessThanToday", "Operation date less than today"));

        // Act
        var result = await _sut.Operation(futureDate, _valid, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(failureExpected);
    }

    [Fact]
    public async Task Operation_InvalidOperation_ReturnsError()
    {
        // Arrange
        var failureExpected = Result.Notify(new Notification(NotificationLevel.Error, "Service.InvalidOperation", "Invalid operation"));

        // Act
        var result = await _sut.Operation(_validDate, _invalid, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(failureExpected);
    }

    [Fact]
    public async Task Operation_ValidParameters_ReturnsSuccess()
    {
        // Arrange
        var resultExpected = Result.Success();

        // Act
        var result = await _sut.Operation(_validDate, _valid, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(resultExpected);
    }

    [Fact]
    public async Task Operation_CancellationRequested_ThrowsTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        Func<Task> act = async () => await _sut.Operation(_validDate, _valid, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
    }
}
