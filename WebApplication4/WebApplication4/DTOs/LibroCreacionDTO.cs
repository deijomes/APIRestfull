using System.ComponentModel.DataAnnotations;
using WebApplication4.validaciones;

namespace WebApplication4.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        [Required]
        public String Titulo { get; set; }
        public DateTime Fechapublicacion { get; set; }
        

        
        public List<int> AutoresIds { get; set;}
    }
}
