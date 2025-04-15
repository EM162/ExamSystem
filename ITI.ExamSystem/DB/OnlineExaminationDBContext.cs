﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Models;

public partial class OnlineExaminationDBContext : DbContext
{
    public OnlineExaminationDBContext()
    {
    }

    public OnlineExaminationDBContext(DbContextOptions<OnlineExaminationDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Intake> Intakes { get; set; }

    public virtual DbSet<IntakeBranchTrackUser> IntakeBranchTrackUsers { get; set; }

    public virtual DbSet<PublishedExam> PublishedExams { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionChoice> QuestionChoices { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserExam> UserExams { get; set; }

    public virtual DbSet<UsersExamsQuestion> UsersExamsQuestions { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=OnlineExaminationDB;Integrated Security=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchID).HasName("PK__Branch__A1682FA555BC9467");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseID).HasName("PK__Courses__C92D718780B52891");

            entity.HasMany(d => d.Tracks).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseTrack",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CourseTracks_Track"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CourseTracks_Course"),
                    j =>
                    {
                        j.HasKey("CourseID", "TrackID").HasName("PK__CourseTr__DE8A3E0B4E998204");
                        j.ToTable("CourseTracks");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersCourse",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersCourses_User"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersCourses_Course"),
                    j =>
                    {
                        j.HasKey("CourseID", "UserID").HasName("PK__UsersCou__1855FD4DC353C6D1");
                        j.ToTable("UsersCourses");
                    });
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamID).HasName("PK__Exams__297521A7696E1DDD");

            entity.Property(e => e.ExamDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Course).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Exams__CourseID__40C49C62");

            entity.HasMany(d => d.Questions).WithMany(p => p.Exams)
                .UsingEntity<Dictionary<string, object>>(
                    "ExamQuestion",
                    r => r.HasOne<Question>().WithMany()
                        .HasForeignKey("QuestionID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ExamQuestions_Question"),
                    l => l.HasOne<Exam>().WithMany()
                        .HasForeignKey("ExamID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ExamQuestions_Exam"),
                    j =>
                    {
                        j.HasKey("ExamID", "QuestionID").HasName("PK__ExamQues__F9A9275FF4688832");
                        j.ToTable("ExamQuestions");
                    });
        });

        modelBuilder.Entity<Intake>(entity =>
        {
            entity.HasKey(e => e.IntakeID).HasName("PK__Intake__7E1E28554E39452C");
        });

        modelBuilder.Entity<IntakeBranchTrackUser>(entity =>
        {
            entity.HasKey(e => new { e.IntakeID, e.BranchID, e.TrackID, e.UserID }).HasName("PK__IntakeBr__3FA3A6DB65D1BE0E");

            entity.HasOne(d => d.Branch).WithMany(p => p.IntakeBranchTrackUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YBT_Branch");

            entity.HasOne(d => d.Intake).WithMany(p => p.IntakeBranchTrackUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YBT_Year");

            entity.HasOne(d => d.Track).WithMany(p => p.IntakeBranchTrackUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YBT_Track");

            entity.HasOne(d => d.User).WithMany(p => p.IntakeBranchTrackUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YBT_Manager");
        });

        modelBuilder.Entity<PublishedExam>(entity =>
        {
            entity.HasKey(e => e.PublishedExamID).HasName("PK__Publishe__D6338807982EE270");

            entity.Property(e => e.PublishDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Branch).WithMany(p => p.PublishedExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishedExams_Branch");

            entity.HasOne(d => d.Course).WithMany(p => p.PublishedExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishedExams_Course");

            entity.HasOne(d => d.Exam).WithMany(p => p.PublishedExams).HasConstraintName("FK_PublishedExams_Exam");

            entity.HasOne(d => d.Intake).WithMany(p => p.PublishedExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishedExams_Intake");

            entity.HasOne(d => d.Track).WithMany(p => p.PublishedExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishedExams_Track");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionID).HasName("PK__Question__0DC06F8C60492EEC");

            entity.HasOne(d => d.Topic).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Topic");
        });

        modelBuilder.Entity<QuestionChoice>(entity =>
        {
            entity.HasKey(e => e.ChoiceID).HasName("PK__Question__76F516867C6897C3");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionChoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionChoice_Question");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleID).HasName("PK__Role__8AFACE3A40E8C610");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicID).HasName("PK__Topics__022E0F7DAAE83A79");

            entity.HasOne(d => d.Course).WithMany(p => p.Topics)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Topics__CourseID__373B3228");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.TrackID).HasName("PK__Track__7A74F8C031737D9F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK__Users__1788CCAC9B6E0D48");

            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_User"),
                    j =>
                    {
                        j.HasKey("UserID", "RoleID").HasName("PK__UserRole__AF27604F5D72267B");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<UserExam>(entity =>
        {
            entity.HasKey(e => new { e.ExamID, e.UserID }).HasName("PK__UserExam__F80DAD6D70CA2EF0");

            entity.HasOne(d => d.Exam).WithMany(p => p.UserExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserExam_Exam");

            entity.HasOne(d => d.User).WithMany(p => p.UserExams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserExam_User");
        });

        modelBuilder.Entity<UsersExamsQuestion>(entity =>
        {
            entity.HasKey(e => new { e.ExamID, e.UserID, e.QuestionID });

            entity.HasOne(d => d.Exam).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentAnswers_Exam");

            entity.HasOne(d => d.Question).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentAnswers_Question");

            entity.HasOne(d => d.User).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentAnswers_Student");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}