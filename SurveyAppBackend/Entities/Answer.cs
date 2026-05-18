namespace SurveyAppBackend.Entities
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ResponseId { get; set; }
        public Response Response { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public Guid? OptionId { get; set; }
        public Option? Option { get; set; }

        public string? TextValue { get; set; }
    }
}
