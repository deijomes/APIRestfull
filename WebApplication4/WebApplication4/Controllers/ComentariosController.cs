using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.DTOs;
using WebApplication4.Entidades;
using WebApplication4.Migrations;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(AplicationDbContext context, IMapper mapper, UserManager<IdentityUser>userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }




        [HttpGet(Name = "obtnerComentarioLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var comentarios = await context.Comentarios
                .Where(comentarioDB => comentarioDB.Id == libroId).ToListAsync();

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }


        [HttpGet("{id:int}", Name = "obtnerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Getporid(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> post(int libroId, comentarioCreacioDTO comentarioCreacioDTO)
        {
            var EmailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "Emmail").FirstOrDefault();
            var Emmail = EmailClaim.Value;
            var Usuario = await userManager.FindByEmailAsync(Emmail);
            var UsuarioId = Usuario.Id;

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacioDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = UsuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtnerComentario", new { id = comentario.Id, libroId }, comentarioDTO);


        }

        [HttpPut("{id:int}", Name ="actualizarComentario")]

        public async Task<ActionResult> put( int libroId, int id, comentarioCreacioDTO comentarioCreacioDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!existeComentario)
            {
                return NotFound();

            }

            var comentario = mapper.Map<Comentario>(comentarioCreacioDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
