using AutoMapper;
using GradingTool.Application.DTOs;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.SumCriteriaScore,
                opt => opt.MapFrom(src => src.Criteria.Sum(c => c.MaxScore)))
            .ForMember(dest => dest.IsBalanced,
                opt => opt.MapFrom(src => src.Criteria.Sum(c => c.MaxScore) == src.MaxScore));

        CreateMap<CreateQuestionDto, Question>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Criteria, opt => opt.Ignore());

        CreateMap<UpdateQuestionDto, Question>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Criteria, opt => opt.Ignore());

        CreateMap<Criterion, CriterionDto>();

        CreateMap<CreateCriterionDto, Criterion>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Question, opt => opt.Ignore());

        CreateMap<UpdateCriterionDto, Criterion>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore());

        CreateMap<Submission, SubmissionListItemDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Submission, SubmissionDetailDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ImageCount, opt => opt.Ignore());

        CreateMap<Grade, GradeDto>()
            .ForMember(dest => dest.CriterionCode,
                opt => opt.MapFrom(src => src.Criterion.Code));
    }
}
