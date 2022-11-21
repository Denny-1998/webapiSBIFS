using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public GroupController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet(Name = "ReadGroups"), Authorize(Roles = "user")]
        public async Task<ActionResult<List<Group>>> GetGroups()
        {
            int userID = _userService.GetUserID();
            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == userID).ToListAsync();
            return Ok(groups);
        }

        [HttpPost(Name = "Create"), Authorize(Roles = "user")]
        public async Task<ActionResult<List<Group>>> Create()
        {
            int userID = _userService.GetUserID();

            Group g = new Group();
            g.OwnerID = userID;

            await _context.Groups.AddAsync(g);
            await _context.SaveChangesAsync();

            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == userID).ToListAsync();

            return new ObjectResult(groups) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
