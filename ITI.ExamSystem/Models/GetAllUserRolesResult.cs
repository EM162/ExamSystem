﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI.ExamSystem.Models
{
    public partial class GetAllUserRolesResult
    {
        public int UserID { get; set; }
        [StringLength(100)]
        public string FullName { get; set; }
        public int RoleID { get; set; }
        [StringLength(50)]
        public string RoleName { get; set; }
    }
}
