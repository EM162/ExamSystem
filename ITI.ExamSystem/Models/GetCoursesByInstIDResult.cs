﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class GetCoursesByInstIDResult
    {
        public int CourseID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int Duration { get; set; }
        [StringLength(255)]
        public string CourseImagePath { get; set; }
    }
}
