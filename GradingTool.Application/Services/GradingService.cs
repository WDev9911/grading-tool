using AutoMapper;
using GradingTool.Application.Common.Exceptions;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Services;

public class GradingService : IGradingService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly ICriterionRepository _criterionRepository;
    private readonly IMapper _mapper;

    public GradingService(
        ISubmissionRepository submissionRepository,
        IGradeRepository gradeRepository,
        IQuestionRepository questionRepository,
        ICriterionRepository criterionRepository,
        IMapper mapper)
    {
        _submissionRepository = submissionRepository;
        _gradeRepository = gradeRepository;
        _questionRepository = questionRepository;
        _criterionRepository = criterionRepository;
        _mapper = mapper;
    }

    public async Task<SubmissionGradingDto> GetGradingDetailAsync(int submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission", submissionId);

        var questions = (await _questionRepository.GetAllWithCriteriaAsync()).ToList();
        var grades = await _gradeRepository.GetBySubmissionIdAsync(submissionId);
        var gradeMap = grades.ToDictionary(g => g.CriterionId);

        return BuildGradingDto(submission, questions, gradeMap);
    }

    public async Task<GradeDto> UpsertGradeAsync(int submissionId, int criterionId, UpsertGradeDto dto)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Status == SubmissionStatus.Graded)
            throw new ConflictException("Bài đã chấm xong, không thể sửa điểm. Reset trước nếu muốn chấm lại");

        var criterion = await _criterionRepository.GetByIdAsync(criterionId)
            ?? throw new NotFoundException("Criterion", criterionId);

        if (dto.Score > criterion.MaxScore)
            throw new BadRequestException(
                $"Điểm {dto.Score} vượt quá điểm tối đa của tiêu chí này ({criterion.MaxScore})");

        var grade = await _gradeRepository.GetBySubmissionAndCriterionAsync(submissionId, criterionId);
        var now = DateTime.UtcNow;

        if (grade == null)
        {
            grade = new Grade
            {
                SubmissionId = submissionId,
                CriterionId = criterionId,
                Score = dto.Score,
                Comment = dto.Comment,
                UpdatedAt = now
            };
            await _gradeRepository.AddAsync(grade);
        }
        else
        {
            grade.Score = dto.Score;
            grade.Comment = dto.Comment;
            grade.UpdatedAt = now;
            await _gradeRepository.UpdateAsync(grade);
        }

        submission.TotalScore = await _gradeRepository.GetTotalScoreAsync(submissionId);
        if (submission.Status == SubmissionStatus.NotGraded)
            submission.Status = SubmissionStatus.Grading;
        await _submissionRepository.UpdateAsync(submission);

        return new GradeDto
        {
            Id = grade.Id,
            SubmissionId = grade.SubmissionId,
            CriterionId = grade.CriterionId,
            CriterionCode = criterion.Code,
            Score = grade.Score,
            Comment = grade.Comment,
            UpdatedAt = grade.UpdatedAt
        };
    }

    public async Task UpdateGeneralCommentAsync(int submissionId, string? comment)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Status == SubmissionStatus.Graded)
            throw new ConflictException("Bài đã chấm xong, không thể sửa. Reset trước nếu muốn chấm lại");

        submission.GeneralComment = comment;
        if (submission.Status == SubmissionStatus.NotGraded)
            submission.Status = SubmissionStatus.Grading;

        await _submissionRepository.UpdateAsync(submission);
    }

    public async Task<SubmissionGradingDto> MarkAsGradedAsync(int submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Status == SubmissionStatus.Graded)
            throw new ConflictException("Bài đã chấm xong rồi");

        var questions = (await _questionRepository.GetAllWithCriteriaAsync()).ToList();
        var totalCriteria = questions.Sum(q => q.Criteria.Count);
        var gradedCount = await _gradeRepository.GetGradeCountAsync(submissionId);

        if (gradedCount < totalCriteria)
            throw new BadRequestException(
                $"Chưa chấm đủ tiêu chí. Còn thiếu {totalCriteria - gradedCount} tiêu chí.");

        submission.TotalScore = await _gradeRepository.GetTotalScoreAsync(submissionId);
        submission.Status = SubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;
        await _submissionRepository.UpdateAsync(submission);

        var grades = await _gradeRepository.GetBySubmissionIdAsync(submissionId);
        var gradeMap = grades.ToDictionary(g => g.CriterionId);
        return BuildGradingDto(submission, questions, gradeMap);
    }

    public async Task ResetGradesAsync(int submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId)
            ?? throw new NotFoundException("Submission", submissionId);

        await _gradeRepository.DeleteBySubmissionIdAsync(submissionId);

        submission.TotalScore = null;
        submission.Status = SubmissionStatus.NotGraded;
        submission.GradedAt = null;
        await _submissionRepository.UpdateAsync(submission);
    }

    private static SubmissionGradingDto BuildGradingDto(
        Submission submission,
        List<Question> questions,
        Dictionary<int, Grade> gradeMap)
    {
        var questionDtos = questions
            .OrderBy(q => q.OrderIndex)
            .Select(q =>
            {
                var criterionDtos = q.Criteria
                    .OrderBy(c => c.OrderIndex)
                    .Select(c =>
                    {
                        gradeMap.TryGetValue(c.Id, out var grade);
                        return new CriterionGradingDto
                        {
                            CriterionId = c.Id,
                            Code = c.Code,
                            Name = c.Name,
                            MaxScore = c.MaxScore,
                            Score = grade?.Score,
                            Comment = grade?.Comment,
                            UpdatedAt = grade?.UpdatedAt
                        };
                    }).ToList();

                var gradedCriteria = criterionDtos.Where(c => c.Score.HasValue).ToList();
                decimal? achieved = gradedCriteria.Count > 0
                    ? gradedCriteria.Sum(c => c.Score!.Value)
                    : null;

                return new QuestionGradingDto
                {
                    QuestionId = q.Id,
                    QuestionNo = q.QuestionNo,
                    Title = q.Title,
                    MaxScore = q.MaxScore,
                    AchievedScore = achieved,
                    Criteria = criterionDtos
                };
            }).ToList();

        return new SubmissionGradingDto
        {
            SubmissionId = submission.Id,
            StudentCode = submission.StudentCode,
            StudentName = submission.StudentName,
            Status = submission.Status.ToString(),
            TotalScore = submission.TotalScore,
            MaxTotalScore = questions.Sum(q => q.MaxScore),
            GeneralComment = submission.GeneralComment,
            GradedAt = submission.GradedAt,
            Questions = questionDtos
        };
    }
}
