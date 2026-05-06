using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAppBackend.DTOs.Option;
using SurveyAppBackend.DTOs.Question;
using SurveyAppBackend.DTOs.Survey;
using SurveyAppBackend.DTOs.User;
using SurveyAppBackend.Entities;
using SurveyAppBackend.Helpers;

namespace SurveyAppBackend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly DBContext _context;

        public SurveyController(DBContext context)
        {
            _context = context;
        }

        [HttpPost("create_survey")]
        [Authorize]
        public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyDto dto)
        {
            if (dto == null) return BadRequest("Sent body is empty");

            if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");

            if (dto.Questions == null) return BadRequest("Required at least one question");

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();


            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Questions = dto.Questions.Select((question, index) => new Question
                {
                    Id = Guid.NewGuid(),
                    Content = question.Content,
                    Type = question.Type,
                    Order = index,
                    Options = question.Options?.Select(o => new Option
                    {
                        Id = Guid.NewGuid(),
                        Content = o
                    }).ToList() ?? new List<Option>()
                }).ToList()
            };
            try {
                _context.Surveys.Add(survey);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while creating survey");
            }


            return Ok(new {survey.Title});
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetSurveyById(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (id == Guid.Empty) return BadRequest("Invalid ID");

            var survey = await _context.Surveys
                .Where(survey => survey.Id == id)
                .Select(survey => new SurveyDto
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    Questions = survey.Questions
                        .OrderBy(question => question.Order)
                        .Select(question => new QuestionDto
                        {
                            Id = question.Id,
                            Content = question.Content,
                            Type = question.Type,
                            Options = question.Options.Select(option => new OptionDto
                            {
                                Id = option.Id,
                                Content = option.Content
                            }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (survey == null) return NotFound();
            return Ok(survey);
        }

        [HttpGet("all_surveys_by_user_id")]
        [Authorize]
        public async Task<IActionResult> GetAllSurveysByUserId()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();


            var surveys = await _context.Surveys
                .Where(survey => survey.UserId == userId)
                .OrderByDescending(survey => survey.CreatedAt)
                .Select(survey => new AllSurveysDto
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    CreatedAt = survey.CreatedAt
                }).ToListAsync();

            if (surveys == null) return NotFound();
            return Ok(surveys);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSurveyById(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (id == Guid.Empty) return BadRequest("Invalid ID");

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(survey => survey.Id == id);

            if (survey == null) return NotFound();
            if (survey.UserId != userId) return Forbid();

            _context.Surveys.Remove(survey);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Error while removing survey");
            }

            return Ok();
        }
    }
    
}
