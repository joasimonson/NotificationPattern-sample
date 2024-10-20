public enum NotificationLevel
{
    None,
    Error,
    Warning
}

public sealed record Notification(NotificationLevel Level, string Code, string? Description = null)
{
    public static readonly Notification None = new(NotificationLevel.None, string.Empty, string.Empty);

    public static implicit operator Result(Notification notification) => Result.Notify(notification);
}

public class Result
{
    private Result(List<Notification> notifications) => AddNotifications(notifications);
    private Result(Notification notification) => AddNotification(notification);

    public void AddNotification(Notification notification)
    {
        switch (notification.Level)
        {
            case NotificationLevel.Error:
                Errors.Add(notification);
                break;
            case NotificationLevel.Warning:
                Warnings.Add(notification);
                break;
            default:
                break;
        }
    }

    public void AddNotifications(List<Notification> notifications)
    {
        Errors.AddRange(notifications.FindAll(n => n.Level == NotificationLevel.Error));
        Warnings.AddRange(notifications.FindAll(n => n.Level == NotificationLevel.Warning));
    }

    public bool IsSuccess => Errors.Count == 0;
    public bool IsFailure => Errors.Count != 0;

    public List<Notification> Errors { get; } = [];
    public List<Notification> Warnings { get; } = [];

    public static Result Success() => new(Notification.None);
    public static Result Success(List<Notification> notifications)
    {
        if (notifications.Any(n => n.Level == NotificationLevel.Error))
            throw new ArgumentException("Invalid error notification in the list", nameof(notifications));

        return new(notifications);
    }

    public static Result Notify(Notification notification) => new(notification);
    public static Result Notify(List<Notification> notifications) => new(notifications);
}