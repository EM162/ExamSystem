﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class sp_ShowUsersExamsQuestionsResult
    {
        public int ExamID { get; set; }
        public int UserID { get; set; }
        public int QuestionID { get; set; }
        [StringLength(2147483647)]
        public string StudentAnswer { get; set; }
        [Column("StudentScore", TypeName = "decimal(5,2)")]
        public decimal? StudentScore { get; set; }
    }
}
