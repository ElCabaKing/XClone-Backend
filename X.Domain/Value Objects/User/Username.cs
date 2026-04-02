

public class Username
{
    public string Value { get; private set; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty.");

        if (value.Length < 3 || value.Length > 20)
            throw new ArgumentException("Username must be between 3 and 20 characters.");

        Value = value;
    }
}