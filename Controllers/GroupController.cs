using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using webapiSBIFS.DataTransferObjects;
using Activity = webapiSBIFS.Model.Activity;

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


        


        [HttpPost("ReadOne"), Authorize(Roles = "user")]                                             //TODO change to get later
        public async Task<ActionResult<Group>> Get(GroupDto requested)
        {
            int userID = _userService.GetUserID();
            var group = await _context.Groups
                .Where(g => g.GroupID == requested.GroupID)
                .Include(g => g.Participants)
                .Include(g => g.Activities)
                .FirstAsync();
            if (group == null)
                return BadRequest("No such group.");

            return Ok(group);
        }


        [HttpPost("ReadMany"), Authorize(Roles = "user")]                                           //TODO change to get later
        public async Task<ActionResult<List<Group>>> Get()
        {
            int userID = _userService.GetUserID();
            // Necessity for a group name which is returned instead? 
            List<Group> groups = await _context.Groups
                .Where(g => g.OwnerID == userID)
                .Include(g => g.Participants)
                .Include(g => g.Activities)
                .ToListAsync();
            return Ok(groups);
        }

        [HttpPost("Create"), Authorize(Roles = "user")]
        public async Task<ActionResult<List<Group>>> Create()
        {
            int userID = _userService.GetUserID();
            var user = await _context.Users.FindAsync(userID);
            if (user == null)
                return BadRequest("No such user.");

            Group group = new Group();
            user.Groups.Add(group);
            group.Participants.Add(user);
            group.OwnerID = userID;

            await _context.Groups.AddAsync(group);
            
            await _context.SaveChangesAsync();

            // Necessity for a group name which is returned instead? 
            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == userID).ToListAsync();

            return new ObjectResult(groups) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpDelete("Delete"), Authorize(Roles = "user")]
        public async Task<ActionResult> Delete(GroupDto requested) 
        {
            int userID = _userService.GetUserID();
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == requested.GroupID && g.OwnerID == userID);
            if (group == null)
                return BadRequest("No such group.");

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AddUser"), Authorize(Roles = "user")]
        public async Task<ActionResult<List<User>>> AddUser(GroupUserDto request)
        {


            //find group in db
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == request.GroupID);
            
            //check if user is owner of the given group
            if (group.OwnerID != _userService.GetUserID())
                return Forbid();

            
            //check if group exists
            if (group == null)
                return BadRequest("No such group.");


            //find user in db
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return BadRequest("User does not exist. ");


            //add user to group
            group.Participants.Add(user);
            _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("Close"), Authorize(Roles = "user")]
        public async Task<ActionResult> CloseGroup(GroupDto requested)
        {
            int userID = _userService.GetUserID();


            var group = await _context.Groups.Include(g => g.Activities).Include(g => g.Participants).
                FirstOrDefaultAsync(g => g.GroupID == requested.GroupID && g.OwnerID == userID);
            var activities = _context.Activities.Include(a => a.Participants).FirstOrDefault();

            



            if (group == null)
                return BadRequest("No such group.");



            //generate a balance for every user
            List<UserBalance> userTotals = new List<UserBalance>();

            foreach (User u in group.Participants)
            {
                UserBalance ub = new UserBalance(u.Email, 0);
                userTotals.Add(ub);
            }


            //calculate total amount for every user and every activity
            List<ActivityReceipt> activityReceipts = new List<ActivityReceipt>();

            foreach (Activity a in group.Activities)
            {
                if (a.Participants.Count == 0)
                    return StatusCode(500, $"An error occured while deviding an activity: \nid: {a.ActivityID} \ndescription: {a.Description} \namount: {a.Amount}");
                
                double amountPerUser = a.Amount / a.Participants.Count;

                List<UserBalance> userBalance = new List<UserBalance>();
                foreach (User u in a.Participants)
                {
                    //show amount in a list in activities
                    UserBalance ub = new UserBalance(u.Email, amountPerUser);
                    userBalance.Add(ub);

                    //add amount to user total amount
                    userTotals.Find(t => t.userEmail == u.Email).balance += amountPerUser;
                }

                ActivityReceipt activityReceipt = new ActivityReceipt(a, userBalance);
                activityReceipts.Add(activityReceipt);
            }



            //finally delete group
            Delete(requested);
            _context.SaveChangesAsync();



            return Ok(new { activityReceipts, userTotals });
        }


    }
}
