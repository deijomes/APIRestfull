using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.DTOs;
using WebApplication4.Entidades;


namespace WebApplication4.Controllers
{
    [Route("api/autores4")]
    [ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(AplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("IConfiguraciones")]
        public ActionResult<string> ObtenerConfiguracion()
        {
            return configuration["ConnectionStrings:defaultconnection"];
        }




        [HttpGet(Name = "obtnerAutores")]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpGet("{id:int}", Name ="obtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);
        }

        [HttpGet("{nombre}", Name ="obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> get(string nombre)
        {
            var autores= await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

           
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post(AutorCreacioDTO autorCreacioDTO)
        {
            var existeAutroConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacioDTO.Nombre);

            if (existeAutroConElMismoNombre)
            {
                return BadRequest($"ya existe un autor con el mismo nombre {autorCreacioDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacioDTO);


            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO= mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new{id= autor.Id  }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")] // api/autores4/1 
        public async Task<ActionResult> Put(AutorCreacioDTO autorCreacioDTO, int id)
        {
           
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound(); 
            }

            var autor= mapper.Map<Autor>(autorCreacioDTO);
            autor.Id = id;
            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id:int}", Name = "BorrarAutor")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
