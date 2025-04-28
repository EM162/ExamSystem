using AutoMapper;
using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.Mapping
{
    public class StuduentProfileAutoMapper :Profile
    {

        public StuduentProfileAutoMapper() {

            
            CreateMap<User, StudentProfileDTO>()
         .ForMember(dest => dest.branchName, op => op.MapFrom(
             src => src.IntakeBranchTrackUsers.FirstOrDefault().Branch.BranchName))
         .ForMember(dest => dest.ProfileImagePath, opt => opt.MapFrom(src => src.ProfileImagePath))
         .ForMember(dest => dest.Intack, op => op.MapFrom(src => src.IntakeBranchTrackUsers.FirstOrDefault().Intake.IntakeName))
         .ForMember(dest => dest.trackname, op => op.MapFrom(
             src => src.IntakeBranchTrackUsers.FirstOrDefault().Track.TrackName))
             .ForMember(dest => dest.userID, opt => opt.MapFrom(src => src.UserID));



            // course and topics mapping
            CreateMap<Course, StudentCoursesDTO>()
             .ForMember(dest => dest.CourseName, op => op.MapFrom(src => src.Name))
             .ForMember(dest => dest.CourseImagePath, opt => opt.MapFrom(src => src.CourseImagePath));

            //.ForMember(dest => dest.Grade, op => op.MapFrom(src => src.grade));

            CreateMap<Topic,StudentTopicsDTO>();  




        }





    }
}
