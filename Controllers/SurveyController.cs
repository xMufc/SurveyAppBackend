using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAppBackend.DTOs.Option;
using SurveyAppBackend.DTOs.Question;
using SurveyAppBackend.DTOs.Response;
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


        /// <summary>
        /// Endpoint to create a new survey. Requires authentication.
        /// </summary>
        /// <param name="dto">The survey data transfer object containing the survey details.</param>
        /// <returns>Result of the operation.</returns>
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

        /// <summary>
        /// Endpoint to get a survey by its ID
        /// </summary>
        /// <param name="id">The ID of the survey to retrieve.</param>
        /// <returns>The survey details if found; otherwise, an appropriate error response.</returns>
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
                            Options = question.Options
                            .Select(option => new OptionDto
                            {
                                Id = option.Id,
                                Content = option.Content
                            })                            
                            .ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (survey == null) return NotFound();
            return Ok(survey);
        }

        /// <summary>
        /// Endpoint to get all surveys created by the authenticated user, ordered by creation date in descending order.
        /// </summary>
        /// <returns>A list of surveys created by the authenticated user.</returns>
        [HttpGet("all_surveys_by_user_id")]
        [Authorize]
        public async Task<IActionResult> GetAllSurveysByUserId()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();


            var surveys = await _context.Surveys
                .Where(survey => survey.UserId == userId)
                .OrderByDescending(survey => survey.CreatedAt)
                .Select(survey => new AllUserSurveysDto
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    CreatedAt = survey.CreatedAt
                }).ToListAsync();

            if (surveys == null) return NotFound();
            return Ok(surveys);
        }

        /// <summary>
        /// Endpoint to delete a survey by its ID. Only the creator of the survey can delete it.
        /// </summary>
        /// <param name="id">The ID of the survey to delete.</param>
        /// <returns>Result of the operation.</returns>
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

        /// <summary>
        /// Endpoint to get all surveys in the system, ordered by creation date in descending order. Requires authentication.
        /// </summary>
        /// <returns>A list of all surveys in the system.</returns>
        [HttpGet("all_surveys")]
        [Authorize]
        public async Task<IActionResult> GetAllSurveys()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();


            var surveys = await _context.Surveys
                .OrderByDescending(survey => survey.CreatedAt)
                .Select(survey => new AllSurveysDto
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    CreatedAt = survey.CreatedAt
                }).ToListAsync();

            if (surveys == null) return NotFound();
            return Ok(surveys);
        }

        /// <summary>
        /// Submits a user's responses to a survey.
        /// </summary>
        /// <param name="dto">An object containing the survey identifier and the user's answers to the survey questions. 
        /// <returns> Result of the operation. </returns>

        [HttpPost("post_response_to_survey")]
        [Authorize]
        public async Task<IActionResult> PostResponseToSurvey([FromBody] SubmitResponseDto dto)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (dto.SurveyId == Guid.Empty) return BadRequest("SurveyID is required");

            var answers = new List<Answer>();

            foreach (var answerDto in dto.Answers)
            {
                if (answerDto.OptionIds != null && answerDto.OptionIds.Any())
                {
                    foreach (var optionId in answerDto.OptionIds)
                    {
                        answers.Add(new Answer
                        {
                            Id = Guid.NewGuid(),
                            QuestionId = answerDto.QuestionId,
                            OptionId = optionId
                        });
                    }
                }
                else
                {
                    answers.Add(new Answer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = answerDto.QuestionId,
                        TextValue = answerDto.TextValue
                    });
                }
            }
            var response = new Response
            {
                Id = Guid.NewGuid(),
                SurveyId = dto.SurveyId,
                CreatedAt = DateTime.UtcNow,
                Answers = answers
            };

            _context.Responses.Add(response);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Error while saving response");
            }

            return Ok();

        }

        /// <summary>
        /// Endpoint providing survey results
        /// </summary>
        /// <param name="surveyId">Survey ID</param>
        /// <returns>List or sum of answers to given survey</returns>
        [HttpGet("results/{surveyId}")]
        [Authorize]
        public async Task<IActionResult> GetSurveyResultsById(Guid surveyId)
        {
            var userId = User.GetUserId();

            if (userId == null)
                return Unauthorized();

            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Include(s => s.Responses)
                    .ThenInclude(r => r.Answers)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                return NotFound();

            if (survey.UserId != userId)
                return Forbid();

            var result = new SurveyResultsDto
            {
                SurveyId = survey.Id,
                Title = survey.Title,
                ResponsesCount = survey.Responses.Count,
                Questions = survey.Questions
                    .OrderBy(question => question.Order)
                    .Select(question =>
                    {
                        if (question.Type == "text")
                        {
                            return new QuestionResultDto
                            {
                                QuestionId = question.Id,
                                Content = question.Content,
                                Type = question.Type,

                                TextAnswers = survey.Responses
                                    .SelectMany(r => r.Answers)
                                    .Where(a =>
                                        a.QuestionId == question.Id &&
                                        !string.IsNullOrWhiteSpace(a.TextValue))
                                    .Select(a => a.TextValue!)
                                    .ToList()
                            };
                        }
                        return new QuestionResultDto
                        {
                            QuestionId = question.Id,
                            Content = question.Content,
                            Type = question.Type,

                            Options = question.Options.Select(option =>
                                new OptionResultDto
                                {
                                    OptionId = option.Id,
                                    Content = option.Content,

                                    Votes = survey.Responses
                                        .SelectMany(r => r.Answers)
                                        .Count(a => a.OptionId == option.Id)
                                })
                                .ToList()
                        };
                    }).ToList()
            };
            return Ok(result);
        }

    }
    
}
