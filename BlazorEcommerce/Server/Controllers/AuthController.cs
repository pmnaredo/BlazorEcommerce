using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister request)
        {
            var user = new User
            {
                Email = request.Email
            };
            var response = await _authService.Register(user, request.Password);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }



    }
}
