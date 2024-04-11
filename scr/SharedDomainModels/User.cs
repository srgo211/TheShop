using SharedInterfaces;

namespace SharedDomainModels;

public class User : IUser
{
    public Guid Guid { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public TupeRole TupeRole { get; set; }
}