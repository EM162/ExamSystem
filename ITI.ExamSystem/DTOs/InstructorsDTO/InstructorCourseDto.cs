using ITI.ExamSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class InstructorCourseDto
    {
        public int Duration { get; set; }

        public string CourseImagePath { get; set; }
        public int CourseID { get; set; }
        public string Name { get; set; }
        public int StudentCount { get; set; }
        public int TopicCount { get; set; }
        public int ExamCount { get; set; }
        public int PublishedExamCount { get; set; }
    }

}
