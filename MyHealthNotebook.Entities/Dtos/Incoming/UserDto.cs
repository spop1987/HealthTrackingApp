namespace MyHealthNotebook.Entities.Dtos.Incoming
{
    public class UserDto : IUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}