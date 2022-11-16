using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapiSBIFS.Model;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<int>> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("User not found.");

            if (user.Password != request.Password)
                return BadRequest("Wrong password.");

            return Ok(user.Privilege);
        }
    }
}
