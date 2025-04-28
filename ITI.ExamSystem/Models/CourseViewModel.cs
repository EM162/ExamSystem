using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ITI.ExamSystem.Models
{
    public class CourseViewModel
    {

        public int CourseID { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string Name { get; set; }

        public int Duration { get; set; }

        [Display(Name = "Course Image")]
        [ValidateNever]
        public IFormFile CourseImage { get; set; }
        [ValidateNever]
        public string ExistingImagePath { get; set; }  // For edit view

        [Display(Name = "Assigned Tracks")]
        public List<int> SelectedTrackIDs { get; set; } = new();
        public List<TrackItem> AvailableTracks { get; set; } = new();

        public class TrackItem
        {
            public int TrackID { get; set; }
            public string TrackName { get; set; }
        }
    }
}
