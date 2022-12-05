﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using webapiSBIFS.DataTransferObjects;

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
    }
}
