using SurveyAppBackend.DTOs.Option;

namespace SurveyAppBackend.DTOs.Question
{
    public class QuestionResultDto
    {
        public Guid QuestionId { get; set; }

        public string Content { get; set; } = null!;

        public string Type { get; set; } = null!;

        public List<OptionResultDto>? Options { get; set; }

        public List<string>? TextAnswers { get; set; }
    }
}
