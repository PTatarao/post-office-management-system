namespace PostOffice.Domain.Entities;

public class PostOffice
{
     public PostOffice(string zipCode, string name, string city)
    {
        Id = Guid.NewGuid();
        SetDetails(zipCode, name, city);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string ZipCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public void Update(string zipCode, string name, string city) => SetDetails(zipCode, name, city);

    private void SetDetails(string zipCode, string name, string city)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        { 
            throw new ArgumentException("ZIP code is required.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Post office name is required.");
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City is required.");
        }

        ZipCode = zipCode.Trim();
        Name = name.Trim();
        City = city.Trim();
    }
}
