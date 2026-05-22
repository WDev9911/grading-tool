using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;

namespace GradingTool.Application.Services;

public class RubricValidationService : IRubricValidationService
{
    private readonly IQuestionRepository _questionRepository;

    public RubricValidationService(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<RubricValidationDto> ValidateAsync()
    {
        var questions = (await _questionRepository.GetAllWithCriteriaAsync()).ToList();
        var errors = new List<string>();

        var questionValidations = questions.Select(q =>
        {
            var sumCriteria = q.Criteria.Sum(c => c.MaxScore);
            var isBalanced = sumCriteria == q.MaxScore;

            if (!isBalanced)
                errors.Add($"Question {q.QuestionNo} '{q.Title}': sum of criteria ({sumCriteria}) ≠ maxScore ({q.MaxScore})");

            return new QuestionValidationDto
            {
                QuestionId = q.Id,
                QuestionNo = q.QuestionNo,
                Title = q.Title,
                QuestionMaxScore = q.MaxScore,
                SumCriteriaScore = sumCriteria,
                IsBalanced = isBalanced
            };
        }).ToList();

        var totalScore = questions.Sum(q => q.MaxScore);
        if (totalScore != 10)
            errors.Add($"Total rubric score is {totalScore}, expected 10");

        return new RubricValidationDto
        {
            TotalScore = totalScore,
            Questions = questionValidations,
            Errors = errors,
            IsValid = errors.Count == 0
        };
    }
}
