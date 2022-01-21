namespace MyHealthNotebook.Entities.Dtos.Incoming
{
    public interface IUserDto
    {
        string FirstName {get; set;}
        string LastName {get; set;}
        string Email {get; set;}
    }
}