using GradingTool.API.Common;
using GradingTool.Application.DTOs;
using GradingTool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradingTool.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    /// <summary>Upload 1 hoặc nhiều file .docx cùng lúc</summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
    public async Task<ActionResult<ApiResponse<BatchUploadResultDto>>> Upload(
        [FromForm] List<IFormFile> files,
        [FromForm] bool overwrite = false)
    {
        var uploadedFiles = files.Select(f => new UploadedFile
        {
            FileName = f.FileName,
            Size = f.Length,
            Stream = f.OpenReadStream()
        }).ToList();

        var result = await _submissionService.UploadAsync(uploadedFiles, overwrite);
        return Ok(ApiResponse<BatchUploadResultDto>.Ok(result));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SubmissionListItemDto>>>> GetAll(
        [FromQuery] SubmissionFilterDto filter)
    {
        var result = await _submissionService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SubmissionListItemDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SubmissionDetailDto>>> GetById(int id)
    {
        var submission = await _submissionService.GetByIdAsync(id)
            ?? throw new GradingTool.Application.Common.Exceptions.NotFoundException("Submission", id);
        return Ok(ApiResponse<SubmissionDetailDto>.Ok(submission));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _submissionService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/content")]
    public async Task<ActionResult<ApiResponse<SubmissionContentDto>>> GetContent(int id)
    {
        var result = await _submissionService.GetContentAsync(id);
        return Ok(ApiResponse<SubmissionContentDto>.Ok(result));
    }

    [HttpGet("{id:int}/images")]
    public async Task<ActionResult<ApiResponse<List<SubmissionImageDto>>>> GetImages(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _submissionService.GetImagesMetadataAsync(id, baseUrl);
        return Ok(ApiResponse<List<SubmissionImageDto>>.Ok(result));
    }

    [HttpGet("{id:int}/images/{fileName}")]
    public async Task<IActionResult> GetImage(int id, string fileName)
    {
        var (stream, contentType, _) = await _submissionService.GetImageStreamAsync(id, fileName);
        Response.Headers["Cache-Control"] = "private, max-age=3600";
        Response.Headers["Content-Disposition"] = "inline";
        return File(stream, contentType, enableRangeProcessing: true);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var (stream, fileName) = await _submissionService.GetOriginalFileStreamAsync(id);
        return File(stream,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            fileName);
    }
}
