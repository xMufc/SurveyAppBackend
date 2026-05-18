using Backend.Services;
using FluentAssertions;
using SurveyAppBackend.Tests.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace SurveyAppBackend.Tests.Services;

public class JwtTokenGeneratorTests
{
    private readonly JwtConfigBuilder _configBuilder = new();
    private readonly UserBuilder _userBuilder = new();

    public class WhenTokenIsGeneratedSuccessfully : JwtTokenGeneratorTests
    {
        private readonly JwtSecurityToken _parsed;

        public WhenTokenIsGeneratedSuccessfully()
        {
            var user = _userBuilder.WithId("abc-42").WithEmail("jan@example.pl").Build();
            var config = _configBuilder.WithIssuer("MyIssuer").WithAudience("MyAudience").Build();

            var raw = JwtService.GenerateJwtToken(user, config);
            _parsed = new JwtSecurityTokenHandler().ReadJwtToken(raw);
        }

        [Fact]
        public void Token_ShouldBeReadable()
            => _parsed.Should().NotBeNull();

        [Fact]
        public void Token_ShouldContainNameIdentifierClaim()
            => _parsed.Claims
                .Should().ContainSingle(c =>
                    c.Type == ClaimTypes.NameIdentifier &&
                    c.Value == "abc-42");

        [Fact]
        public void Token_ShouldContainEmailClaim()
            => _parsed.Claims
                .Should().ContainSingle(c =>
                    c.Type == ClaimTypes.Email &&
                    c.Value == "jan@example.pl");

        [Fact]
        public void Token_ShouldHaveCorrectIssuer()
            => _parsed.Issuer.Should().Be("MyIssuer");

        [Fact]
        public void Token_ShouldHaveCorrectAudience()
            => _parsed.Audiences.Should().Contain("MyAudience");
    }
    public class WhenCheckingExpiration : JwtTokenGeneratorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        [InlineData(24)]
        public void Token_ShouldExpireAfterConfiguredHours(int hours)
        {
            var config = _configBuilder.WithExpireHours(hours.ToString()).Build();
            var before = DateTime.UtcNow;

            var raw = JwtService.GenerateJwtToken(_userBuilder.Build(), config);
            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(raw);

            parsed.ValidTo
                .Should().BeCloseTo(
                    nearbyTime: before.AddHours(hours),
                    precision: TimeSpan.FromSeconds(5));
        }
    }
    public class WhenConfigurationIsInvalid : JwtTokenGeneratorTests
    {
        [Fact]
        public void Token_ShouldThrow_WhenKeyIsMissing()
        {
            var config = _configBuilder.WithoutKey().Build();
            var user = _userBuilder.Build();

            var act = () => JwtService.GenerateJwtToken(user, config);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Token_ShouldThrow_WhenExpireHoursIsNotANumber()
        {
            var config = _configBuilder.WithExpireHours("nie-liczba").Build();
            var user = _userBuilder.Build();

            var act = () => JwtService.GenerateJwtToken(user, config);

            act.Should().Throw<FormatException>()
               .WithMessage("*not in a correct format*");
        }
    }
}