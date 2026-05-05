namespace SurveyAppBackend.Entities
{
    public class Response
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
