using AutoMapper;
using GradingTool.Application.Common.Exceptions;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Services;

public class CriterionService : ICriterionService
{
    private readonly ICriterionRepository _criterionRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public CriterionService(
        ICriterionRepository criterionRepository,
        IQuestionRepository questionRepository,
        IMapper mapper)
    {
        _criterionRepository = criterionRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CriterionDto>> GetByQuestionIdAsync(int questionId)
    {
        _ = await _questionRepository.GetByIdAsync(questionId)
            ?? throw new NotFoundException("Question", questionId);

        var criteria = await _criterionRepository.GetByQuestionIdAsync(questionId);
        return _mapper.Map<IEnumerable<CriterionDto>>(criteria);
    }

    public async Task<CriterionDto> CreateAsync(int questionId, CreateCriterionDto dto)
    {
        _ = await _questionRepository.GetByIdAsync(questionId)
            ?? throw new NotFoundException("Question", questionId);

        if (await _criterionRepository.ExistsByCodeAsync(questionId, dto.Code))
            throw new ConflictException($"Code '{dto.Code}' already exists in this Question");

        var criterion = _mapper.Map<Criterion>(dto);
        criterion.QuestionId = questionId;
        await _criterionRepository.AddAsync(criterion);

        return _mapper.Map<CriterionDto>(criterion);
    }

    public async Task<CriterionDto> UpdateAsync(int id, UpdateCriterionDto dto)
    {
        var criterion = await _criterionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Criterion", id);

        if (await _criterionRepository.ExistsByCodeAsync(criterion.QuestionId, dto.Code, id))
            throw new ConflictException($"Code '{dto.Code}' already exists in this Question");

        if (criterion.MaxScore != dto.MaxScore && await _criterionRepository.HasGradesAsync(id))
            throw new ConflictException("Cannot change MaxScore of Criterion that already has grades");

        _mapper.Map(dto, criterion);
        await _criterionRepository.UpdateAsync(criterion);

        return _mapper.Map<CriterionDto>(criterion);
    }

    public async Task DeleteAsync(int id)
    {
        var criterion = await _criterionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Criterion", id);

        if (await _criterionRepository.HasGradesAsync(id))
            throw new ConflictException("Cannot delete Criterion that already has grades");

        await _criterionRepository.DeleteAsync(criterion);
    }
}
