﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Models;

[Table("Track")]
public partial class Track
{
    [Key]
    public int TrackID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string TrackName { get; set; }

    [InverseProperty("Track")]
    public virtual ICollection<IntakeBranchTrackUser> IntakeBranchTrackUsers { get; set; } = new List<IntakeBranchTrackUser>();

    [InverseProperty("Track")]
    public virtual ICollection<PublishedExam> PublishedExams { get; set; } = new List<PublishedExam>();

    [ForeignKey("TrackID")]
    [InverseProperty("Tracks")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}