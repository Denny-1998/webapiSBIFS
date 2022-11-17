using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using webapiSBIFS.Tools;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string tokenSaltPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\SBIFS\\salt.txt";

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

            user = await _context.Users.FirstOrDefaultAsync(u => u.Password == request.Password);
            if (user != null)
                return UnprocessableEntity("Password not valid.");

            User u = new User(request.Email, request.Password);

            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();

            string token = CreateToken(u);
            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Forbid("Wrong username or password.");

            if (user.Password != request.Password)
                return Forbid("Wrong username or password.");

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User u)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Role, u.Privilege.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(new TextFile().GetAllTextFromFile(tokenSaltPath)));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
