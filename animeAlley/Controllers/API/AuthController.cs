using animeAlley.Models.ViewModels.AuthenticatorDTO;
using animeAlley.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace animeAlley.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas ou conta não ativa" });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            if (response == null)
            {
                return BadRequest(new { message = "Erro ao criar conta. Usuário ou email já existem." });
            }

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var utilizador = await _authService.GetUtilizadorByUserNameAsync(userName);
            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = utilizador.Id,
                nome = utilizador.Nome,
                userName = utilizador.UserName,
                foto = utilizador.Foto,
                banner = utilizador.Banner,
                isAdmin = utilizador.isAdmin,
            });
        }
    }
}
