using System.ComponentModel.DataAnnotations;
using WebApplication4.validaciones;

namespace WebApplication4.DTOs
{
    public class AutorCreacioDTO
    {
        [Required(ErrorMessage = "el campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "el campo{0} no debe tener mas de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string? Nombre { get; set; }
    }
}
