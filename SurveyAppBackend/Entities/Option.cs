namespace SurveyAppBackend.Entities
{
    public class Option
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
