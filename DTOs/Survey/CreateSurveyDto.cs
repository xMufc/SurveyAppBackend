using SurveyAppBackend.DTOs.Question;

namespace SurveyAppBackend.DTOs.Survey
{
    public class CreateSurveyDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public List<CreateQuestionDto> Questions { get; set; } = new();

    }
}
