using Microsoft.Extensions.Configuration;

namespace SurveyAppBackend.Tests.Helpers;

public class JwtConfigBuilder
{
    private string _key = "super-secret-key-that-is-long-enough-32ch";
    private string _issuer = "TestIssuer";
    private string _audience = "TestAudience";
    private string _expireHours = "1";

    public JwtConfigBuilder WithKey(string key) { _key = key; return this; }
    public JwtConfigBuilder WithIssuer(string issuer) { _issuer = issuer; return this; }
    public JwtConfigBuilder WithAudience(string audience) { _audience = audience; return this; }
    public JwtConfigBuilder WithExpireHours(string hours) { _expireHours = hours; return this; }
    public JwtConfigBuilder WithoutKey() { _key = null!; return this; }

    public IConfiguration Build()
    {
        var settings = new Dictionary<string, string?>
        {
            { "JwtConfig:Issuer",      _issuer      },
            { "JwtConfig:Audience",    _audience    },
            { "JwtConfig:ExpireHours", _expireHours }
        };

        if (_key is not null)
            settings["JwtConfig:Key"] = _key;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}