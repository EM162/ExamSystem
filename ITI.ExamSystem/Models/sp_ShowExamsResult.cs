﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class sp_ShowExamsResult
    {
        public int ExamID { get; set; }
        public int? CourseID { get; set; }
        [StringLength(50)]
        public string ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
        public int MCQQuestionCount { get; set; }
        public int TrueFalseQuestionCount { get; set; }
        public int Duration { get; set; }
    }
}
