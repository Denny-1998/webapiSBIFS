using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using webapiSBIFS.DataTransferObjects;
using webapiSBIFS.Model;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {

        #region values and constructors 
        private readonly DataContext _context;
        private readonly IUserService _userService;



        public AdminUserController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        #endregion





        #region User Methods




        [HttpDelete("DeleteUser"), Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteUser(UserDto userRequest)
        {

            var user = await _context.Users.FindAsync(userRequest.Email);
            if (user == null)
                return BadRequest("No such user.");

            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == user.UserID).ToListAsync();

            foreach (Group group in groups)
            {
                _context.Groups.Remove(group);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }





        [HttpGet("CountUsers"), Authorize(Roles = "admin")]
        public async Task<ActionResult> CountUsers()
        {
            int count = await _context.Users.CountAsync();

            return Ok(new { count });
        }


        #endregion
    }
}
