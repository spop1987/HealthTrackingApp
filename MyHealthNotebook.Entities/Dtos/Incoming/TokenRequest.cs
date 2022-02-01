using System.ComponentModel.DataAnnotations;

namespace MyHealthNotebook.Entities.Dtos.Incoming
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}