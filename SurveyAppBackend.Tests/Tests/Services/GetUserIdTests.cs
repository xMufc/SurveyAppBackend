using FluentAssertions;
using SurveyAppBackend.Helpers;
using System.Security.Claims;
using Xunit;

namespace SurveyAppBackend.Tests.Services;

public class GetUserIdTests
{
    private static ClaimsPrincipal BuildPrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        return new ClaimsPrincipal(identity);
    }


    public class WhenUserHasNameIdentifierClaim : GetUserIdTests
    {
        [Fact]
        public void GetUserId_ShouldReturnUserId()
        {
            var user = BuildPrincipal(
                new Claim(ClaimTypes.NameIdentifier, "user-123")
            );

            user.GetUserId().Should().Be("user-123");
        }

        [Fact]
        public void GetUserId_ShouldReturnCorrectId_WhenMultipleClaimsPresent()
        {
            var user = BuildPrincipal(
                new Claim(ClaimTypes.Email, "jan@example.pl"),
                new Claim(ClaimTypes.NameIdentifier, "user-456"),
                new Claim(ClaimTypes.Role, "Admin")
            );

            user.GetUserId().Should().Be("user-456");
        }
    }


    public class WhenUserHasNoNameIdentifierClaim : GetUserIdTests
    {
        [Fact]
        public void GetUserId_ShouldReturnNull_WhenClaimIsMissing()
        {
            var user = BuildPrincipal(
                new Claim(ClaimTypes.Email, "jan@example.pl")
            );

            user.GetUserId().Should().BeNull();
        }

        [Fact]
        public void GetUserId_ShouldReturnNull_WhenNoClaims()
        {
            var user = BuildPrincipal();

            user.GetUserId().Should().BeNull();
        }

        [Fact]
        public void GetUserId_ShouldReturnNull_WhenPrincipalHasNoIdentity()
        {
            var user = new ClaimsPrincipal();

            user.GetUserId().Should().BeNull();
        }
    }


    public class WhenClaimValueIsEdgeCase : GetUserIdTests
    {
        [Fact]
        public void GetUserId_ShouldReturnEmptyString_WhenClaimValueIsEmpty()
        {
            var user = BuildPrincipal(
                new Claim(ClaimTypes.NameIdentifier, string.Empty)
            );

            user.GetUserId().Should().BeEmpty();
        }

        [Theory]
        [InlineData("123")]
        [InlineData("guid-f47ac10b-58cc-4372-a567-0e02b2c3d479")]
        [InlineData("very-long-id-" + "x")]
        public void GetUserId_ShouldReturnExactValue_ForVariousIdFormats(string userId)
        {
            var user = BuildPrincipal(
                new Claim(ClaimTypes.NameIdentifier, userId)
            );

            user.GetUserId().Should().Be(userId);
        }
    }
}