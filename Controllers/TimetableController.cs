using Exam_Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam_Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {

        private readonly UserContext _userContext;

        public TimetableController(UserContext userContext)
        {
            _userContext = userContext;
        }

        private List<TimetableEntry> GenerateTimetableLogic(int userId, DateTime startDate, DateTime endDate, List<Exam> exams, UserContext context)
        {
            List<TimetableEntry> timetable = new List<TimetableEntry>();
            int hallCount = 5;
            int maxOccupancyPerHall = 40;
            int examCount = exams.Count;

            // Calculate the number of days in the date range
            int totalDays = (int)(endDate - startDate).TotalDays + 1;

            // Ensure the date range is within the allowed limits
            if (totalDays < 3 || totalDays > 20)
            {
                // Handle invalid date range
                return timetable;
            }

            // Assign each exam to the user
            List<int> assignedExams = new List<int>();
            for (int i = 0; i < examCount; i++)
            {
                assignedExams.Add(i);
            }

            // Generate timetables for each day
            for (int day = 0; day < totalDays; day++)
            {
                // Distribute exams across halls
                for (int hallId = 1; hallId <= hallCount; hallId++)
                {
                    int examsPerHall = examCount / hallCount; // Distribute exams equally among halls

                    for (int i = 0; i < examsPerHall; i++)
                    {
                        if (assignedExams.Count == 0)
                        {
                            break; // All exams assigned
                        }

                        // Randomly select an exam from the remaining ones
                        int randomIndex = new Random().Next(0, assignedExams.Count);
                        int examIndex = assignedExams[randomIndex];
                        assignedExams.RemoveAt(randomIndex);

                        // Get the selected exam
                        Exam selectedExam = exams[examIndex];

                        // Calculate the start time for the exam (between 8 AM and 5 PM)
                        DateTime startTime = startDate.Date.AddHours(8).AddDays(day);
                        DateTime endTime = startTime.AddMinutes(selectedExam.DurationMinutes);

                        // Check if the end time exceeds 5 PM, if so, move to the next day
                        if (endTime.Hour >= 17)
                        {
                            continue;
                        }

                        // Check hall occupancy for the selected time slot
                        int occupiedCount = context.TimetableEntries
                            .Count(te => te.DateTime >= startTime && te.DateTime < endTime && te.HallId == hallId);

                        if (occupiedCount < maxOccupancyPerHall)
                        {
                            // Create a timetable entry for the exam
                            TimetableEntry entry = new TimetableEntry
                            {
                                UserId = userId,
                                ExamId = selectedExam.ExamId,
                                HallId = hallId,
                                DateTime = startTime
                            };

                            // Add the entry to the timetable
                            timetable.Add(entry);
                        }
                    }
                }
            }

            return timetable;
        }


        [HttpPost("generate-all-timetables")]
        public async Task<ActionResult> GenerateAllTimetables()
        {
            try
            {
                // Fetch all users from the database
                var users = await _userContext.Users.ToListAsync();

                // Fetch the list of exams from the database
                var exams = await _userContext.Exams.ToListAsync();

                foreach (var user in users)
                {
                    // Check if timetables for this user already exist in the database
                    var existingTimetables = await _userContext.TimetableEntries
                        .Where(te => te.UserId == user.UserId)
                        .ToListAsync();

                    if (existingTimetables.Count == 0)
                    {
                        // Generate timetables for this user
                        List<TimetableEntry> timetable = GenerateTimetableLogic(
                            user.UserId,
                            user.StartDate, 
                            user.EndDate,   
                            exams,
                            _userContext);

                        // Store timetable entries in the database
                        foreach (var entry in timetable)
                        {
                            _userContext.TimetableEntries.Add(entry);
                        }
                        await _userContext.SaveChangesAsync();
                    }
                }

                return Ok("Timetables generated for all users.");
            }
            catch (Exception ex)
            {
                // Log the exception or use a debugger to inspect the error details
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("timetables/{id}")]
        public async Task<ActionResult<IEnumerable<TimetableEntry>>> GetTimetablesByUserId(int id)
        {
            try
            {
                var timetables = await _userContext.TimetableEntries
                .Where(te => te.UserId == id)
                .ToListAsync();

                if (timetables == null || timetables.Count == 0)
                {
                    return NotFound();
                }
                return Ok(timetables);
            }
            catch (Exception ex)
            {
                // Log the exception or use a debugger to inspect the error details
                return StatusCode(500, ex.Message);
            }

        }

    }
}
