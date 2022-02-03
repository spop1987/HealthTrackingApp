namespace MyHealthNotebook.Entities.Dtos.Incoming
{
    public class UpdateProfileDto
    {
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
    }
}