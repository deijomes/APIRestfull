using System.ComponentModel.DataAnnotations;
using WebApplication4.validaciones;

namespace WebApplication4.DTOs
{
    public class LibroPaTchDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        [Required]
        public string? Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
    }
}
