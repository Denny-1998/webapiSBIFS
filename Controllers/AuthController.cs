using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using webapiSBIFS.Tools;

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

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
                return UnprocessableEntity("Email not valid or taken.");
            
            string salt = new SaltAdapter().GetSalt();
            string hashedPass = SecurityTools.HashString(request.Password, salt);

            user = await _context.Users.FirstOrDefaultAsync(u => u.Password == hashedPass);
            if (user != null)
                return UnprocessableEntity("Password not valid.");

            User u = new User(request.Email, hashedPass);

            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();

            string token = JwtTools.CreateToken(u);
            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Forbid("Wrong username or password.");

            string salt = new SaltAdapter().GetSalt();
            string hashedPass = SecurityTools.HashString(request.Password, salt);

            if (user.Password != hashedPass)
                return Forbid("Wrong username or password.");

            string token = JwtTools.CreateToken(user);
            return Ok(token);
        }
    }
}
