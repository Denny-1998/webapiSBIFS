﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using webapiSBIFS.DataTransferObjects;
using webapiSBIFS.Tools;
using static webapiSBIFS.Model.User;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;


        public AuthController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }






        // Test method for reading claims
        [HttpGet, Authorize(Roles = "admin")]
        public ActionResult<object> GetMe()
        {
            var userID = _userService.GetUserID();
            return Ok(new { userID });

            //var userID = User?.Identity?.Name;
            //var userRole = User?.FindFirstValue(ClaimTypes.Role);
            //return Ok(new {userID, userRole});
        }




        /// <summary>
        /// method for creating a new user 
        /// creating an admin is not possible yet, this has to be changed through MS SQL server management studio
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<ActionResult> Register(AuthDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
                return UnprocessableEntity("Email not valid or taken.");
            
            string salt = new SaltAdapter().GetSalt();
            string hashedPass = SecurityTools.HashString(request.Password, salt);

            user = await _context.Users.FirstOrDefaultAsync(u => u.Password == hashedPass);
            if (user != null)
                return UnprocessableEntity("Password not valid.");

            User u = new User(request.Email, hashedPass);

            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();

            string token = JwtTools.CreateToken(u);
            return NoContent();
        }





        /// <summary>
        /// method for login, checks if username exists and password is right and returns a jwt token 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login(AuthDto request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return StatusCode(401, "User name does not exist. ");

            string salt = new SaltAdapter().GetSalt();
            string hashedPass = SecurityTools.HashString(request.Password, salt);

            if (user.Password != hashedPass)
                return StatusCode(401, "Wrong password.");


            var jwt = JwtTools.CreateToken(user);
            string role = user.Privilege.ToString();


            
            return Ok(new {jwt, role});
        }
    }
}
