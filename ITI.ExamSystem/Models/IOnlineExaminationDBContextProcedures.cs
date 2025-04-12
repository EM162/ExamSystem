﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ITI.ExamSystem.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ITI.ExamSystem.Models
{
    public partial interface IOnlineExaminationDBContextProcedures
    {
        Task<int> assign_user_to_courseAsync(int? userid, int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> CorrectExamAsync(int? examID, int? userID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<CreateCourseResult>> CreateCourseAsync(string name, int? duration, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> Delete_CourseTrackAsync(int? courseid, int? trackid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteCourseAsync(int? courseId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteCourses_ByTrackIdAsync(int? trackid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteQuestionAsync(int? questionID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteQuestionChoiceAsync(int? choiceID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteRoleAsync(int? roleId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteTopicAsync(int? topicID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteUserAsync(int? userId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> DeleteUserRoleAsync(int? userId, int? roleId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> GenerateExamQuestionsAsync(int? examID, int? mCQCount, int? tFCount, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<get_all_instructorsResult>> get_all_instructorsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<get_all_students_with_coursesResult>> get_all_students_with_coursesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<get_courses_with_student_countResult>> get_courses_with_student_countAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<get_students_not_in_courseResult>> get_students_not_in_courseAsync(int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAll_courseTracksResult>> GetAll_courseTracksAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAll_Student_ByCourseIDResult>> GetAll_Student_ByCourseIDAsync(int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAll_Student_ByCourseNameResult>> GetAll_Student_ByCourseNameAsync(string courseName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllCoursesResult>> GetAllCoursesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllCoursesByinstIDResult>> GetAllCoursesByinstIDAsync(int? instId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllQuestionChoiceResult>> GetAllQuestionChoiceAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllQuestionsResult>> GetAllQuestionsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllRolesResult>> GetAllRolesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllTopicsResult>> GetAllTopicsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllUserRolesResult>> GetAllUserRolesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetAllUsersResult>> GetAllUsersAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetBranchByIDResult>> GetBranchByIDAsync(int? branchID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetBranchesResult>> GetBranchesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetBranchesByTrackResult>> GetBranchesByTrackAsync(int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetBranchesTracksByIntakeResult>> GetBranchesTracksByIntakeAsync(int? intakeID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetCourseByIDResult>> GetCourseByIDAsync(int? courseId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetCourses_ByTrackIdResult>> GetCourses_ByTrackIdAsync(int? trackid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetCoursesByInstIDResult>> GetCoursesByInstIDAsync(int? userId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetCoursesByUserIDResult>> GetCoursesByUserIDAsync(int? userId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetCourseTopicsResult>> GetCourseTopicsAsync(int? courseID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetExamQuestionsAndChoicesResult>> GetExamQuestionsAndChoicesAsync(int? examID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetExamStudentAnswersResult>> GetExamStudentAnswersAsync(int? examID, int? studentID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetInstructor_ByCourseIDResult>> GetInstructor_ByCourseIDAsync(int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetInstructorCoursesWithStudentCountResult>> GetInstructorCoursesWithStudentCountAsync(int? instructorID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetIntakeBranchTrackUserByIDResult>> GetIntakeBranchTrackUserByIDAsync(int? intakeID, int? branchID, int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetIntakeBranchTrackUsersResult>> GetIntakeBranchTrackUsersAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetIntakeByIDResult>> GetIntakeByIDAsync(int? intakeID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetIntakesResult>> GetIntakesAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetRoleByIdResult>> GetRoleByIdAsync(int? roleId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetStudentCourseGradesResult>> GetStudentCourseGradesAsync(int? studentID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetStudentsByDepartmentResult>> GetStudentsByDepartmentAsync(int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetTrackByIDResult>> GetTrackByIDAsync(int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetTracksResult>> GetTracksAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetTracks_ByCourseIdResult>> GetTracks_ByCourseIdAsync(int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetTracksByBranchResult>> GetTracksByBranchAsync(int? branchID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetUserByIdResult>> GetUserByIdAsync(int? userId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetUserRolesByUserIDResult>> GetUserRolesByUserIDAsync(int? userId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetUsersByIntakeBranchTrackResult>> GetUsersByIntakeBranchTrackAsync(int? intakeID, int? branchID, int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> Insert_CourseTrackAsync(int? courseid, int? trackid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertBranchAsync(string branchName, string address, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertIntakeAsync(string intakeName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertIntakeBranchTrackUserAsync(int? intakeID, int? branchID, int? trackID, int? userID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertQuestionChoiceAsync(int? questionID, string choiceText, int? correctChoice, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertQuestionsAsync(int? topicID, string questionText, string questionType, decimal? grade, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertRoleAsync(string roleName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertRoleToUserAsync(int? userId, int? roleId, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertTopicAsync(int? courseID, string topicName, string description, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertTrackAsync(string trackName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> InsertUserAsync(string fullName, string email, string passwordHash, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> remove_user_from_courseAsync(int? userid, int? courseid, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> remove_user_from_course_by_roleAsync(int? userid, int? courseid, string rolename, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> SafeDeleteBranchAsync(int? branchID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> SafeDeleteIntakeAsync(int? intakeID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> SafeDeleteIntakeBranchTrackUserAsync(int? intakeID, int? branchID, int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> SafeDeleteTrackAsync(int? trackID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<SearchCoursesByNameResult>> SearchCoursesByNameAsync(string searchTerm, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_DeleteExamAsync(int? examID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_DeleteExamQuestionAsync(int? examID, int? questionID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_DeleteUserExamAsync(int? examID, int? userID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_DeleteUsersExamsQuestionAsync(int? examID, int? userID, int? questionID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_InsertExamAsync(int? courseID, string examType, DateTime? examDate, int? mCQQuestionCount, int? trueFalseQuestionCount, int? duration, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_InsertExamQuestionAsync(int? examID, int? questionID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_InsertUserExamAsync(int? examID, int? userID, int? grade, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_InsertUsersExamsQuestionAsync(int? examID, int? userID, int? questionID, string studentAnswer, decimal? studentScore, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<sp_ShowExamQuestionsResult>> sp_ShowExamQuestionsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<sp_ShowExamsResult>> sp_ShowExamsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<sp_ShowUserExamsResult>> sp_ShowUserExamsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<sp_ShowUsersExamsQuestionsResult>> sp_ShowUsersExamsQuestionsAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_UpdateExamAsync(int? examID, int? courseID, string examType, DateTime? examDate, int? mCQQuestionCount, int? trueFalseQuestionCount, int? duration, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_UpdateExamQuestionAsync(int? examID, int? oldQuestionID, int? newQuestionID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_UpdateUserExamAsync(int? examID, int? userID, int? grade, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_UpdateUsersExamsQuestionAsync(int? examID, int? userID, int? questionID, string studentAnswer, decimal? studentScore, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> SubmitExamAnswerAsync(int? examID, int? userID, int? questionID, string studentAnswer, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateBranchAsync(int? branchID, string branchName, string address, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateCourseAsync(int? courseID, string name, int? duration, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateCourseDurationAsync(int? courseID, int? duration, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateCourseNameAsync(int? courseID, string name, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateIntakeAsync(int? intakeID, string intakeName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateIntakeBranchTrackUserManagerAsync(int? intakeID, int? branchID, int? trackID, int? newUserID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateQuestionAsync(int? questionID, int? topicID, string questionText, string questionType, decimal? grade, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateQuestionChoiceAsync(int? choiceID, int? questionID, string choiceText, int? correctChoice, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateRoleAsync(int? roleId, string roleName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateTopicAsync(int? topicID, int? courseID, string topicName, string description, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateTrackAsync(int? trackID, string trackName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateUserAsync(int? userId, string fullName, string email, string passwordHash, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> UpdateUserRoleAsync(int? userID, int? oldRoleID, int? newRoleID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    }
}
