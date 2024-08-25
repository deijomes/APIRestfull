using AutoMapper;
using WebApplication4.DTOs;
using WebApplication4.Entidades;

namespace WebApplication4.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacioDTO, Autor>();
            CreateMap < Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(AutorDTO => AutorDTO.libro, opciones => opciones.MapFrom(MapAutorDTOlibro));


            CreateMap<LibroCreacionDTO, Libro>()
                      .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
                CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(LibroDTO => LibroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPaTchDTO, Libro>().ReverseMap();


            CreateMap<comentarioCreacioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO > MapAutorDTOlibro(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if(autor.AutoresLibros == null ) { return resultado; }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });

            } 



            return resultado;
        }


        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if ( libro.AutoresLibros == null ) { return resultado; }

            foreach (var autorlibro in libro.AutoresLibros)  
            {
                resultado.Add(new AutorDTO()
                {
                    id = autorlibro.AutorId,
                     nombre = autorlibro.Autor.Nombre 

                });

            }


            return resultado;
        }


        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado= new List<AutorLibro>();
            
            if (libroCreacionDTO.AutoresIds == null) {  return resultado; }

            foreach ( var  autorId  in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });

            }

            return  resultado;
        }
    }
}
