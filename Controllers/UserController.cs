using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapiSBIFS.DataTransferObjects;
using webapiSBIFS.Model;
using webapiSBIFS.Tools;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public UserController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet("Read"), Authorize(Roles = "admin, user")]
        public async Task<ActionResult<object>> Get()
        {
            var userID = _userService.GetUserID();
            string email = string.Empty;
            var query = from u in _context.Users
                        where u.UserID == userID
                        select u.Email;

            email = await query.FirstAsync();

            return Ok(new { email });
        }

        [HttpPut("Update"), Authorize(Roles = "user")]
        public async Task<ActionResult> Update(AuthDto request)
        {
            string hashedPass = string.Empty;
            int userID = _userService.GetUserID();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            
            if (request.Email != null && request.Email != string.Empty)
            {
                var isUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (isUser != null)
                    return UnprocessableEntity("User with given email already exists. Could not update the account.");

                user.Email = request.Email;
            }
            if (request.Password != null && request.Password != string.Empty)
            {
                string salt = new SaltAdapter().GetSalt();
                hashedPass = SecurityTools.HashString(request.Password, salt);

                var isUser = await _context.Users.FirstOrDefaultAsync(u => u.Password == hashedPass);
                if (isUser != null)
                    return UnprocessableEntity("Password not valid.");

                user.Password = hashedPass;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("Delete"), Authorize(Roles = "user")]
        public async Task<ActionResult> Delete()
        {
            int userID = _userService.GetUserID();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user == null)
                return BadRequest("No such user.");

            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == userID).ToListAsync();

            foreach (Group group in groups)
            {
                _context.Groups.Remove(group);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
