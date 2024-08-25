using System.ComponentModel.DataAnnotations;
using WebApplication4.validaciones;


namespace WebApplication4.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="el campo {0} es requerido")]
        [StringLength(maximumLength:100, ErrorMessage = "el campo{0} no debe tener mas de {1} caracteres")]
        [PrimeraLetraMayuscula]
      
        public string ?Nombre { get; set; }
        public List<AutorLibro > AutoresLibros { get; set; }
    }
}
