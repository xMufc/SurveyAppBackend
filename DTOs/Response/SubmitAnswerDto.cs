namespace SurveyAppBackend.DTOs.Response
{
    public class SubmitAnswerDto
    {
        public Guid QuestionId { get; set; }

        public List<Guid>? OptionIds { get; set; }

        public string? TextValue { get; set; }
    }
}
