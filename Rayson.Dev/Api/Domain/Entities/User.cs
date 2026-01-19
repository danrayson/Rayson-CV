namespace Domain.Entities;

public class User : Entity
{
    public required string Email { get; set; }
    public required string UserName { get; set; }
}
