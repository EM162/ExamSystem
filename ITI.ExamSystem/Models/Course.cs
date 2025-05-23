﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Models;

public partial class Course
{
    [Key]
    public int CourseID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public int Duration { get; set; }

    [StringLength(255)]
    public string CourseImagePath { get; set; }
    public bool IsDeleted { get; set; } = false;


    [InverseProperty("Course")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Course")]
    public virtual ICollection<PublishedExam> PublishedExams { get; set; } = new List<PublishedExam>();

    [InverseProperty("Course")]
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();

    [ForeignKey("CourseID")]
    [InverseProperty("Courses")]
    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();

    [ForeignKey("CourseID")]
    [InverseProperty("Courses")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
}