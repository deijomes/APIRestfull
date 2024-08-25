using System.ComponentModel.DataAnnotations;
using WebApplication4.validaciones;

namespace WebApplication4.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        
        public string? Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }

        
        
    }
}
