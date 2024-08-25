using System.ComponentModel.DataAnnotations;

namespace WebApplication4.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; }
    }
}
