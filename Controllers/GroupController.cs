﻿using Microsoft.AspNetCore.Authorization;
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


        

        /// <summary>
        /// get all information about one group belonging to logged in user
        /// </summary>
        /// <param name="requested"></param>
        /// <returns></returns>
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


        /// <summary>
        /// list all groups with information belonging to one user
        /// </summary>
        /// <returns></returns>
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





        /// <summary>
        /// listing all activities in a given group 
        /// checks if group belongs to logged in user
        /// </summary>
        /// <param name="requested"></param>
        /// <returns></returns>
        [HttpPost("GetActivities"), Authorize(Roles = "user")]                                               //TODO change to get later
        public async Task<ActionResult<Group>> GetActivities(GroupDto requested)
        {
            int userID = _userService.GetUserID();

            //get group from db
            var group = await _context.Groups.Include(g => g.Activities).Include(g => g.Participants).FirstOrDefaultAsync(g => g.GroupID == requested.GroupID);
            if (group == null)
                return BadRequest("No such group.");


            //check if user has access to group
            bool isParticipant = false;
            foreach (User u in group.Participants)
            {
                if (u.UserID == userID) 
                    isParticipant = true;
            }

            if (!isParticipant)
                return Forbid("You have no access to the requested group. ");





            //list all activities
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





        /// <summary>
        /// create a new group with the logged in user as owner
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// deletes one group belonging to the logged in user
        /// </summary>
        /// <param name="requested"></param>
        /// <returns></returns>
        [HttpPost("Delete"), Authorize(Roles = "user")]
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



        /// <summary>
        /// adds another participant to a group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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







        /// <summary>
        /// adds an activity to a group
        /// available for users who own the group or who are participants in the group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddActivity"), Authorize(Roles = "user")]
        public async Task<ActionResult<List<User>>> AddActivity(GroupActivityDtoUser request)
        {

            int userID = _userService.GetUserID();

            //find group in db
            var group = await _context.Groups.Include(g => g.Participants).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);

            if (group == null)
                return BadRequest("No such group.");







            //check if user has access to group
            bool isParticipant = false;
            foreach (User u in group.Participants)
            {
                if (u.UserID == userID)
                    isParticipant = true;
            }

            if (!isParticipant)
                return Forbid("You have no access to the requested group. ");




            //find user in db
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userID);

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




        /// <summary>
        /// edit one activity 
        /// only if you are the owner
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("EditActivity"), Authorize(Roles = "user")]
        public async Task<ActionResult> EditActivity(ActivityFullDtoUser request)
        {
            int userID = _userService.GetUserID();

            //find activity and group in db
            Activity activity = await _context.Activities.Include(a => a.Participants).FirstOrDefaultAsync(a => a.ActivityID == request.ActivityID);


            Group group = await _context.Groups.Include(g => g.Participants).Include(g => g.Activities).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);


            //check if activity exists
            if (activity == null)
                return BadRequest("Activity not found. ");

            //check if user is owner of this activity or owner of this group
            //group owners can edit activities
            if (activity.OwnerID != userID && group.OwnerID != userID)
                return Forbid("You are not the owner of this activity or this group. ");




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
            activity.Participants = Participants;
            activity.Group = group;


            _context.SaveChangesAsync();

            return Ok(activity);
        }





        /// <summary>
        /// delete one activity 
        /// only if you are owner of this activity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("DeleteActivity"), Authorize(Roles = "user")]
        public async Task<ActionResult> DeleteActivity(ActivityDto request)
        {
            int userID = _userService.GetUserID();

            //find activity in db and check if user is owner of it
            var activity = await _context.Activities.Include(a => a.Participants)
                .FirstOrDefaultAsync(a => a.ActivityID == request.ActivityID && a.OwnerID == userID);


            //find group in db
            //var group = await _context.Groups.Include(g => g.Activities).FirstOrDefaultAsync(g => g.GroupID == request.GroupID);

            if (activity == null)
                return BadRequest("No such activity or no access. ");




            //remove activity from group
            _context.Activities.Remove(activity);
            _context.SaveChangesAsync();

            return Ok();
        }




        /// <summary>
        /// close a group and calculate the total for each user and each activity 
        /// </summary>
        /// <param name="requested"></param>
        /// <returns>
        /// returns a list of activities with amounts each user has to pay and 
        /// then a total for each user as a list
        /// </returns>
        [HttpPost("Close"), Authorize(Roles = "user")]
        public async Task<ActionResult> CloseGroup(GroupDto requested)
        {
            int userID = _userService.GetUserID();


            var group = await _context.Groups.Include(g => g.Activities).Include(g => g.Participants).
                FirstOrDefaultAsync(g => g.GroupID == requested.GroupID && g.OwnerID == userID);
            var activities = _context.Activities.Include(a => a.Participants).Where(a => a.Group.GroupID == group.GroupID);




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

            foreach (Activity a in activities)
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
            _context.Groups.Remove(group);
            _context.SaveChangesAsync();



            return Ok(new { activityReceipts, userTotals });
        }


    }
}
