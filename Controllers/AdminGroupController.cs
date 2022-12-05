﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using webapiSBIFS.DataTransferObjects;
using webapiSBIFS.Model;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminGroupController : ControllerBase
    {

        #region values and constructors 
        private readonly DataContext _context;
        private readonly IUserService _userService;



        public AdminGroupController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        #endregion




        #region http methods
        [HttpPost("ReadOne"), Authorize(Roles = "admin")]                                               //TODO change to get later
        public async Task<ActionResult<Group>> Get(GroupDto requested)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == requested.GroupID);
            if (group == null)
                return BadRequest("No such group.");

            return Ok(group);
        }




        
        [HttpPost("ReadMany"), Authorize(Roles = "admin")]                                              //TODO change to get later 
        public async Task<ActionResult<List<Group>>> Get(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("No such user.");

            var groups = await _context.Groups.Where(g => g.OwnerID == user.UserID).ToListAsync();
            return Ok(groups);
        }





        [HttpPost("Create"), Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Group>>> Create(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("No such user.");

            Group g = new Group();
            g.OwnerID = user.UserID;

            await _context.Groups.AddAsync(g);
            await _context.SaveChangesAsync();

            // Necessity for a group name which is returned instead? 
            List<Group> groups = await _context.Groups.Where(g => g.OwnerID == user.UserID).ToListAsync();

            return new ObjectResult(groups) { StatusCode = StatusCodes.Status201Created };
        }


        

        [HttpPut("Edit"), Authorize(Roles ="admin")]
        public async Task<ActionResult<List<Group>>> Edit(GroupFullDto request)
        {
            Group groupToEdit = await _context.Groups.FirstOrDefaultAsync(g => g.GroupID == request.GroupID);


            //error handling (might need more)
            if (groupToEdit == null)
                return BadRequest("No such group");
            if (request.OwnerID == null && request.Participants == null && request.Activities == null)
                return BadRequest("no input given");


            //change given values
            if (request.OwnerID != null) 
                groupToEdit.OwnerID = request.OwnerID;
            if (request.Participants != null) 
                groupToEdit.Participants = request.Participants;
            if (request.Activities != null) 
                groupToEdit.Activities = request.Activities;


            //save changes 
            _context.SaveChangesAsync();

            //check if changes are applied
            List<Group> groups = await _context.Groups.Where(g => g.GroupID == request.GroupID).ToListAsync();

            return new ObjectResult(groups) { StatusCode = StatusCodes.Status200OK};
        }

        


        [HttpDelete("Delete"), Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(GroupDto request)
        {
            Group? g = await _context.Groups.FindAsync(request.GroupID);
            if (g == null)
                return BadRequest("No such group.");

            _context.Groups.Remove(g);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion
    }
}
