namespace Exam_Scheduler.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }

        public ICollection<TimetableEntry>? TimetableEntries { get; set; }
    }
}
