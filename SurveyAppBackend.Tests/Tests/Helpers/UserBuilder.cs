using Backend.Entites;

namespace SurveyAppBackend.Tests.Helpers;

public class UserBuilder
{
    private string _id = "user-123";
    private string _email = "test@example.com";

    public UserBuilder WithId(string id) { _id = id; return this; }
    public UserBuilder WithEmail(string email) { _email = email; return this; }

    public User Build() => new User { Id = _id, Email = _email };
}