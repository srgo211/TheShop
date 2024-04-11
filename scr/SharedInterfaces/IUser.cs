namespace SharedInterfaces;

public enum TupeRole
{
    none = 0,
    admin = 2,
    user = 4,
}



public interface IUser
{
    Guid Guid { get; set; }
    long UserId { get; set; }
    string UserName { get; set; }
    string Password { get; set; }
    string Email { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    TupeRole TupeRole { get; set; }
}