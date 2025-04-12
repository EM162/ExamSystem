﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Models;

[Keyless]
public partial class UsersExamsQuestion
{
    public int ExamID { get; set; }

    public int UserID { get; set; }

    public int QuestionID { get; set; }

    public string StudentAnswer { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? StudentScore { get; set; }

    [ForeignKey("ExamID")]
    public virtual Exam Exam { get; set; }

    [ForeignKey("QuestionID")]
    public virtual Question Question { get; set; }

    [ForeignKey("UserID")]
    public virtual User User { get; set; }
}