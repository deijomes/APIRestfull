using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.DTOs;
using WebApplication4.Entidades;

namespace WebApplication4.Controllers
{
    [Route("api/libros4")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtnerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {

            var libro = await context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);


        }

        [HttpPost (Name = "crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear libro sin autores");
            }

            var autoresIds = await context.Autores.
                Where(autorDb => libroCreacionDTO.AutoresIds.Contains(autorDb.Id)).Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("no existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);





            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtnerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name ="actualizarLibro")]

        public async Task<ActionResult> put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                          .Include(x => x.AutoresLibros)
                          .FirstOrDefaultAsync(x => x.Id == id);
            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();
            return NoContent();


        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}", Name ="patchLibro")]

        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPaTchDTO> patchDocument)
        {
            try
            {
                if (patchDocument == null)
                {
                    return BadRequest();
                }

                var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

                if (libroDB == null)
                {
                    return NotFound();

                }
                var libroDTO = mapper.Map<LibroPaTchDTO>(libroDB);

                patchDocument.ApplyTo(libroDTO, ModelState);

                var esValido = TryValidateModel(libroDTO);

                if (!esValido)
                {
                    return BadRequest(ModelState);
                }

                mapper.Map(libroDTO, libroDB);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        [HttpDelete("{id:int}", Name =" borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();



        }

    }
}

