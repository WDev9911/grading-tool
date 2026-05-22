using AutoMapper;
using GradingTool.Application.Common.Exceptions;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Domain.Entities;

namespace GradingTool.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuestionService(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<QuestionDto>> GetAllAsync()
    {
        var questions = await _questionRepository.GetAllWithCriteriaAsync();
        return _mapper.Map<IEnumerable<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> GetByIdAsync(int id)
    {
        var question = await _questionRepository.GetByIdWithCriteriaAsync(id)
            ?? throw new NotFoundException("Question", id);
        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
    {
        if (await _questionRepository.ExistsByQuestionNoAsync(dto.QuestionNo))
            throw new ConflictException($"QuestionNo {dto.QuestionNo} already exists");

        var question = _mapper.Map<Question>(dto);
        await _questionRepository.AddAsync(question);

        var created = await _questionRepository.GetByIdWithCriteriaAsync(question.Id);
        return _mapper.Map<QuestionDto>(created!);
    }

    public async Task<QuestionDto> UpdateAsync(int id, UpdateQuestionDto dto)
    {
        var question = await _questionRepository.GetByIdWithCriteriaAsync(id)
            ?? throw new NotFoundException("Question", id);

        if (await _questionRepository.ExistsByQuestionNoAsync(dto.QuestionNo, id))
            throw new ConflictException($"QuestionNo {dto.QuestionNo} already exists");

        _mapper.Map(dto, question);
        await _questionRepository.UpdateAsync(question);

        var updated = await _questionRepository.GetByIdWithCriteriaAsync(id);
        return _mapper.Map<QuestionDto>(updated!);
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _questionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Question", id);

        if (await _questionRepository.HasGradesAsync(id))
            throw new ConflictException("Cannot delete Question that already has grades");

        await _questionRepository.DeleteAsync(question);
    }
}
