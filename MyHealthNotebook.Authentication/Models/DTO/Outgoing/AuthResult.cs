namespace MyHealthNotebook.Authentication.Models.DTO.Outgoing
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Succes { get; set; }
        public List<string> Errors { get; set; }
    }
}