﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class GetStudentCourseGradesResult
    {
        public int CourseID { get; set; }
        [StringLength(100)]
        public string CourseName { get; set; }
        [Column("TotalScore", TypeName = "decimal(38,2)")]
        public decimal? TotalScore { get; set; }
        [Column("MaxPossibleScore", TypeName = "decimal(38,2)")]
        public decimal? MaxPossibleScore { get; set; }
        [Column("Percentage", TypeName = "decimal(5,2)")]
        public decimal? Percentage { get; set; }
    }
}
