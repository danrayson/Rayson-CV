namespace Database.Exceptions;

public class DatabaseConfigurationException(string message)
    : InvalidOperationException(message)
{
}
