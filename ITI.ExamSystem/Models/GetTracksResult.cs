﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class GetTracksResult
    {
        public int TrackID { get; set; }
        [StringLength(100)]
        public string TrackName { get; set; }
    }
}
