using Exam_Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Exam_Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _userContext;

        public UserController(UserContext userContext)
        {
            _userContext = userContext;
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if(_userContext.Users == null)
            {
                return NotFound();
            }
            return await _userContext.Users.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_userContext.Users == null)
            {
                return NotFound();
            }
            var user = await _userContext.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return user;
        }


        [HttpPost]
        public IActionResult PostUser(int userId, User user)
        {

            try
            {
                User userObj = new User();
                userObj.UserId = userId; // Set the user ID from the parameter
                userObj.StartDate = user.StartDate;
                userObj.EndDate = user.EndDate;

                _userContext.Users.Add(userObj);
                _userContext.SaveChanges();

                return Ok("User created successfully.");
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;

                return BadRequest(errorMessage);
            }
        }

        /*        [HttpPost]
                public async Task<ActionResult<User>> PostUser(User user)
                {
                    _userContext.Users.Add(user);
                    await _userContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
                }*/

        /*        [HttpPost]
                public async Task<ActionResult<User>> PostUser(User user)
                {
                    try
                    {
                        // Check if a user with the same UserId already exists
                        var existingUser = await _userContext.Users.FindAsync(user.UserId);

                        if (existingUser != null)
                        {
                            // If a user with the same UserId exists, return a conflict response
                            return Conflict("A user with the same UserId already exists.");
                        }

                        // Add the user with the provided UserId
                        _userContext.Users.Add(user);
                        await _userContext.SaveChangesAsync();

                        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
                    }
                    catch (DbUpdateException ex)
                    {
                        // Log the exception for debugging purposes
                        Console.WriteLine(ex);

                        // Return an appropriate error response
                        return StatusCode(500, "An error occurred while saving the entity changes.");
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or return an appropriate error response
                        return StatusCode(500, $"An error occurred: {ex.Message}");
                    }
                }*/




        [HttpGet("exams")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExams()
        {
            try
            {
                var exams = await _userContext.Exams.ToListAsync();
                return Ok(exams);
            }
            catch (Exception ex)
            {
                // Log the exception or use a debugger to inspect the error details
                return StatusCode(500, ex.Message);
            }
        }


    }
}
