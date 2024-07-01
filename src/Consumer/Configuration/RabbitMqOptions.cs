namespace Consumer.Configuration;

public record RabbitMqOptions
{
    public static readonly string Section = "RabbitMq";

    public string Host { get; init; } = string.Empty;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Host) ||
            string.IsNullOrEmpty(User) ||
            string.IsNullOrEmpty(Password))
        {
            throw new ArgumentNullException(Section);
        }
    }
}