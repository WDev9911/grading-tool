using FluentValidation;
using FluentValidation.AspNetCore;
using GradingTool.API.Filters;
using GradingTool.API.Middleware;
using GradingTool.Application.Common.Interfaces;
using GradingTool.Application.Common.Settings;
using GradingTool.Application.Mappings;
using GradingTool.Application.Services;
using GradingTool.Application.Services.Interfaces;
using GradingTool.Application.Validators;
using GradingTool.Infrastructure.Persistence;
using GradingTool.Infrastructure.Repositories;
using GradingTool.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Allow large multipart uploads (up to 500 MB)
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 524_288_000);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(CreateQuestionDtoValidator).Assembly);

// Disable default ModelState auto-400 so our ValidatorActionFilter can throw custom ValidationException
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Form size limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524_288_000;
    options.ValueCountLimit = 200;
});

// Storage settings
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));

// Repositories
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ICriterionRepository, CriterionRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();

// Services
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ICriterionService, CriterionService>();
builder.Services.AddScoped<IRubricValidationService, RubricValidationService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IDocxParserService, DocxParserService>();
builder.Services.AddScoped<IDocxImageExtractor, DocxImageExtractor>();
builder.Services.AddScoped<IDocxTextExtractor, DocxTextExtractor>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// CORS — expose Content-Disposition so browsers can read the download filename
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidatorActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure Storage directory exists
var storagePath = Path.GetFullPath(
    builder.Configuration["Storage:BasePath"] ?? "Storage");
Directory.CreateDirectory(storagePath);

// Global exception handler — must be first in pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
