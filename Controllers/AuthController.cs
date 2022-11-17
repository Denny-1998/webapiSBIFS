using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
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

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
                return BadRequest("User account with email already exists.");

            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Password);
            if (user != null)
                return BadRequest("Password taken.");

            User u = new User(request.Email, request.Password);

            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
