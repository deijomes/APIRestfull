using System.ComponentModel.DataAnnotations;

namespace WebApplication4.DTOs
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Emmail { get; set; }
        [Required]
        public string password { get; set; }
    }
}
