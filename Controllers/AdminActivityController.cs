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
    public class AdminActivityController : ControllerBase
    {

        #region values and constructors 

        private readonly DataContext _context;
        private readonly IUserService _userService;



        public AdminActivityController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }



        #endregion

        #region Activities Methods





        [HttpPost("GetActivities"), Authorize(Roles = "admin")]                                               //TODO change to get later
        public async Task<ActionResult<Group>> GetActivities(GroupDto requested)
        {
            var group = await _context.Groups.Include(g => g.Activities).FirstOrDefaultAsync(g => g.GroupID == requested.GroupID);
            if (group == null)
                return BadRequest("No such group.");


            List<ActivityFullDto> activities = new List<ActivityFullDto>();

            foreach (Activity a in group.Activities)
            {
                ActivityFullDto aDto = new ActivityFullDto();

                //set parameters
                aDto.ActivityID = a.ActivityID;
                aDto.Amount = a.Amount;
                aDto.Description = a.Description;
                aDto.OwnerID = a.OwnerID;

                //convert loop to make it more readable
                List<string> ParticipantsEmail = new List<string>();
                foreach (User u in a.Participants)
                {
                    ParticipantsEmail.Add(u.Email);
                }
                aDto.ParticipantsEmail = ParticipantsEmail;

                //get group
                aDto.GroupID = group.GroupID;

                //add to new list 
                activities.Add(aDto);
            }


            return Ok(activities);
        }






        [HttpPost("AddActivity"), Authorize(Roles = "admin")]
        public async Task<ActionResult<List<User>>> AddActivity(GroupActivityDto request)
        {

            //find group in db
            var group = await _context.Groups.Include(g => g.Participants).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);

            if (group == null)
                return BadRequest("No such group.");


            //find user in db
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.OwnerEmail);

            if (user == null)
                return BadRequest("User does not exist. ");

            //for each user in request, check if it is in group
            foreach (string u in request.ParticipantsEmail)
            {
                //check if user is in group
                bool userInGroup = false;
                foreach (User groupParticipants in group.Participants)
                {
                    if (u == groupParticipants.Email) 
                        userInGroup = true;
                }

                //if not, return
                if (!userInGroup)
                    return BadRequest($"At least one of the users is not participant of this group: \n{u}");
            }
            

            //check if group contains desired ActivityOwner
            if (!group.Participants.Contains(user))
                return BadRequest("Desired owner is not participant of this group. ");



            //set all parameters
            Activity activity = new Activity();
            activity.Amount = request.Amount;
            activity.Description = request.Description;
            activity.OwnerID = user.UserID;
            activity.Participants = new List<User>();
            activity.Group = group;

            //find all users in list
            List<string> UserNotFound = new List<string>();
            foreach (string email in request.ParticipantsEmail)
            { 
                var findUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                
                if (findUser != null)
                    activity.Participants.Add(findUser);
                else
                    UserNotFound.Add(email);
            }

            if (UserNotFound.Count() > 0)
                return BadRequest(new { UserNotFound });

            //add user to group
            group.Activities.Add(activity);
            _context.SaveChangesAsync();

            return Ok(group);
        }



        [HttpPut("EditActivity"), Authorize(Roles = "admin")]
        public async Task<ActionResult> EditActivity(ActivityFullDto request)
        {

            //find activity in db
            Activity activity = await _context.Activities.Include(a => a.Participants).FirstOrDefaultAsync(a => a.ActivityID == request.ActivityID);
            Task<Group> getGroup = _context.Groups.Include(g => g.Participants).Include(g => g.Activities).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);


            if (activity == null) 
                return BadRequest("Activity not found. ");


            Group group = getGroup.Result;

            //find user in db
            var desiredOwner = await _context.Users.FirstOrDefaultAsync(u => u.UserID == request.OwnerID);



            List<User> users = group.Participants;


            //for each user in request, check if it is in group
            foreach (string u in request.ParticipantsEmail)
            {
                //check if user is in group
                bool userInGroup = false;
                foreach (User groupParticipants in group.Participants)
                {
                    if (u == groupParticipants.Email)
                        userInGroup = true;
                }

                //if not, return
                if (!userInGroup)
                    return BadRequest($"At least one of the users is not participant of this group: \n{u}");
            }


            //check if group contains desired ActivityOwner
            if (!group.Participants.Contains(desiredOwner))
                return BadRequest("Desired owner is not participant of this group. ");






            //convert user into string to avoid sending uneccessary information
            List<User> Participants = new List<User>();
            foreach (string email in request.ParticipantsEmail)
            {
                User u = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                Participants.Add(u);
            }


            

            //set all parameters
            activity.Amount = request.Amount;
            activity.Description = request.Description;
            activity.OwnerID = request.OwnerID;
            activity.Participants = Participants;
            activity.Group = group;


            _context.SaveChangesAsync();

            return Ok(activity);
        }




        [
            ("DeleteActivity"), Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteActivity(ActivityDto request)
        {
            //find group in db
            var group = await _context.Groups.Include(g => g.Activities).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);

            if (group == null)
                return BadRequest("No such group.");



            //find activity in list
            Activity activityToDelete = null;

            foreach (Activity a in group.Activities)
            {
                if (a.ActivityID == request.ActivityID)
                    activityToDelete = a;
            }

            //check if activity could be found
            if (activityToDelete == null)
                return BadRequest("No activity found. ");
            

            //remove activity from group
            group.Activities.Remove(activityToDelete);
            _context.SaveChangesAsync();

            return Ok();
        }






        #endregion

    }
}
