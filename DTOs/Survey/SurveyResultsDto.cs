using SurveyAppBackend.DTOs.Question;

namespace SurveyAppBackend.DTOs.Survey
{
    public class SurveyResultsDto
    {
        public Guid SurveyId { get; set; }

        public string Title { get; set; } = null!;
        public int ResponsesCount { get; set; }
        public List<QuestionResultDto> Questions { get; set; } = new();
    }
}
