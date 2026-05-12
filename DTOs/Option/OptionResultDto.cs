namespace SurveyAppBackend.DTOs.Option
{
    public class OptionResultDto
    {
        public Guid OptionId { get; set; }

        public string Content { get; set; } = null!;

        public int Votes { get; set; }
    }
}
