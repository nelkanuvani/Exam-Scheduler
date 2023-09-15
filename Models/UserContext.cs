using Microsoft.EntityFrameworkCore;
namespace Exam_Scheduler.Models
     
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }

    }
}

