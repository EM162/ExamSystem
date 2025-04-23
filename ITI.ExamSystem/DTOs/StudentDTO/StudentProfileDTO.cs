using System.Security.Cryptography.X509Certificates;

namespace ITI.ExamSystem.DTOs.StudentDTO
{
    public class StudentProfileDTO
    {
        public int userID { get; set; } 
        public string fullname { get; set; }
        public string Email { get; set; }   
        public string branchName { get; set; }
        public string trackname { get; set; }
       public string Intack {  get; set; } 
        public string? ProfileImagePath { get; set; }
        public List<string> Courses { get; set; }
        



    }
}
