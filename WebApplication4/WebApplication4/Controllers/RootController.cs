using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.DTOs;

namespace WebApplication4.Controllers

{

    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService autorizationService;

        public RootController(IAuthorizationService autorizationService)
        {
            this.autorizationService = autorizationService;
        }

        [HttpGet(Name = "ObtenerRuta")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datoHateoas = new List<DatoHATEOAS>();

            var esAdmin = await autorizationService.AuthorizeAsync(User, "esAdmin");

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRuta", new { }),
                descripcion: " self ", metodo: "GET "));

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtnerAutores", new { }),
              descripcion: " autor", metodo: "GET "));


            if (esAdmin.Succeeded)
            {
                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }),
               descripcion: "autor-crear", metodo: "POST "));

                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-Crear", metodo: "POST "));

            }

            return datoHateoas;
        }

    }
}
