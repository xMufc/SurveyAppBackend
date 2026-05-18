namespace SurveyAppBackend.DTOs.Survey
{
    public class AllSurveysDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { set; get; }

    }
}
