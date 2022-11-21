using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserService _userService;

        public UserController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet(Name = "Read"), Authorize(Roles = "user")]
        public async Task<ActionResult<object>> Get()
        {
            var userID = _userService.GetUserID();
            string email = string.Empty;
            var query = from u in _context.Users
                        where u.UserID == userID
                        select u.Email;

            email = query.FirstAsync().Result;

            return Ok(new { email });
        }

        //[HttpPut(Name = "Update"), Authorize(Roles = "user")]

    }
}
