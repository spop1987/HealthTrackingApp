namespace MyHealthNotebook.Entities.Dtos.Outgoing
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Succes { get; set; }
        public List<string> Errors { get; set; }
    }
}