namespace SurveyAppBackend.Entities
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!; 
        public int Order { get; set; }
        public bool IsRequired { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
