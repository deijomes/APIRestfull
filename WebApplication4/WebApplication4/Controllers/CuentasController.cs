using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication4.DTOs;
using WebApplication4.servicios___Copia;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_quizas_secreto");
        }

        [HttpGet("Hash /{textoPlano}")]
        public ActionResult realizarhash ( string textoPlano)
        
        {
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);

            return Ok ( new {
                textoPlano = textoPlano,
                hash1= resultado1,
                hash2 = resultado2
            });



        }

        [HttpGet("Encriptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "felipe gavilan";
            var textoCifrado =dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano= textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado=textoDesencriptado
            });
        }

        [HttpGet("encriptadoPorTiempo")]
        public ActionResult EncriptadoPorTiempor()
        {
            var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

            var textoPlano = "felipe gavilan";
            var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });
        }



        [HttpPost("registrar", Name ="registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Emmail, Email = credencialesUsuario.Emmail };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.password);

            if (resultado.Succeeded)
            {
                return await ConstruitToken(credencialesUsuario);
            }

            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("Login", Name = "loguinUsuario")]

        public async Task<ActionResult<RespuestaAutenticacion>> login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync
                (credencialesUsuario.Emmail, credencialesUsuario.password, isPersistent: false, lockoutOnFailure: false);


            if (resultado.Succeeded)
            {
                return await ConstruitToken(credencialesUsuario);
            }

            else { return BadRequest("login incorrecto"); }

        }

        [HttpGet("renovarTOken", Name = "renoverToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> renovar()
        {
            var EmailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "Emmail").FirstOrDefault();
            var Emmail = EmailClaim.Value;
            var credencialesUsuario= new CredencialesUsuario()
            {
                Emmail = Emmail,
            };

            return await ConstruitToken (credencialesUsuario);
        }

        



        private  async Task< RespuestaAutenticacion> ConstruitToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim ("Emmail", credencialesUsuario.Emmail),
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Emmail);
            var claimDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimDB);



            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var Expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken=  new JwtSecurityToken(issuer:null, audience:null, claims:claims, expires:Expiracion, signingCredentials: creds );

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken), Expiracion = Expiracion
            };
        }

        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario =  await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "1 "));
            return NoContent();
        }

        [HttpPost("RemoveAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1 "));
            return NoContent();
        }





    }
}
