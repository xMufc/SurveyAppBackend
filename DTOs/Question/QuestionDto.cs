using SurveyAppBackend.DTOs.Option;

namespace SurveyAppBackend.DTOs.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public List<OptionDto> Options { get; set; } = new();
    }
}
