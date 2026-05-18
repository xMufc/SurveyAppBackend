namespace SurveyAppBackend.DTOs.Response
{
    public class SubmitResponseDto
    {
        public Guid SurveyId { get; set; }

        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }

}
