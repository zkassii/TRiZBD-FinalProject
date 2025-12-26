using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Services;

namespace ShoeStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService service) : ControllerBase
    {
        private readonly AuthService _service = service;

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> PostUser(LoginDto loginDto)
        {
            var token = await _service.AuthUserWithTokenAsync(loginDto);

            return token is null ? BadRequest() : Ok(token);
        }
    }
}
