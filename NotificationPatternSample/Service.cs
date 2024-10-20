public sealed class Service
{
    public async Task<Result> Operation(DateTime date, bool valid, CancellationToken cancellationToken)
    {
        var notifications = new List<Notification>();

        if (date > DateTime.Now.Date)
            return ServiceErrors.DateGreaterThanToday;

        await Task.Delay(1000, cancellationToken);

        if (!valid)
            return ServiceErrors.InvalidOperation;

        if (date < DateTime.Now.Date)
            notifications.Add(ServiceWarnings.DateLessThanToday);

        return Result.Success(notifications);
    }
}

public static class ServiceErrors
{
    public static readonly Notification DateGreaterThanToday
        = new(NotificationLevel.Error, "Service.GreaterThanToday", "Operation date greater than today");

    public static readonly Notification InvalidOperation
        = new(NotificationLevel.Error, "Service.InvalidOperation", "Invalid operation");
}

public static class ServiceWarnings
{
    public static readonly Notification DateLessThanToday
        = new(NotificationLevel.Warning, "Service.DateLessThanToday", "Operation date less than today");
}