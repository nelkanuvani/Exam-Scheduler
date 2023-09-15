namespace Exam_Scheduler.Models
{
    public class TimetableEntry
    {
        public int TimetableEntryId { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int HallId { get; set; }
        public DateTime DateTime { get; set; }
        public string ExamName
        {
            get
            {
                if (Exam != null)
                {
                    return Exam.Name; 
                }
                return null;
            }
        }

        public User User { get; set; }
        public Exam Exam { get; set; }
    }
}

