namespace SurveyAppBackend.DTOs.Question
{
    public class CreateQuestionDto
    {
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public List<string>? Options { get; set; }
    }
}
