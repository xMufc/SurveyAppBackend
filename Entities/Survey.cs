using Azure;
using Backend.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyAppBackend.Entities
{
    public class Survey
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Response> Responses { get; set; } = new List<Response>();

    }
}
