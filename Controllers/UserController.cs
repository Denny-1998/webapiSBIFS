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

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
                return BadRequest("User profile with email already exists. Did you forget you password?");

            user = await _context.Users.FirstOrDefaultAsync(u => u.Password == request.Password);
            if (user != null)
                return BadRequest("Password already exists.");

            User u = new User(request.Email, request.Password);
            await _context.AddAsync(u);
            await _context.SaveChangesAsync();

            return Ok(u);
        }
    }
}
