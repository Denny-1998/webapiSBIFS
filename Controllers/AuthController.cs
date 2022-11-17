using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

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
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("User not found.");

            if (user.Password != request.Password)
                return BadRequest("Wrong password.");


            string token = CreateToken(user);
            return Ok(token);
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

            string token = CreateToken(u);
            return Ok(token);
        } 

        private string CreateToken(User u)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Role, u.Privilege.ToString())
            };

            //// Still needs a key, to be figured out //
            //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes())

            return string.Empty;
        }
    }
}
