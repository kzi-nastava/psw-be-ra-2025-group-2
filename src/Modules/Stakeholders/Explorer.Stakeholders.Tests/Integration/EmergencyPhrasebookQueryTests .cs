using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Xunit;

using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos.Emergency;
using Explorer.Stakeholders.API.Public.Emergency;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class EmergencyPhrasebookQueryTests : BaseStakeholdersIntegrationTest
    {
        public EmergencyPhrasebookQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetLanguages_returns_supported_languages()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var result = controller.GetLanguages().ShouldBeOfType<OkObjectResult>();
            var langs = result.Value.ShouldBeAssignableTo<IReadOnlyList<EmergencyPhrasebookLanguageDto>>();

            langs.Count.ShouldBeGreaterThan(0);
            langs.Any(x => x.Code == "en").ShouldBeTrue();
        }

        [Fact]
        public void GetSentences_sr_returns_list_with_texts()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var result = controller.GetSentences("sr").ShouldBeOfType<OkObjectResult>();
            var sentences = result.Value.ShouldBeAssignableTo<IReadOnlyList<EmergencyPhrasebookSentenceDto>>();

            sentences.Count.ShouldBeGreaterThan(0);
            sentences.All(s => !string.IsNullOrWhiteSpace(s.Key)).ShouldBeTrue();
            sentences.All(s => s.Text != null).ShouldBeTrue();
        }

        [Fact]
        public void GetSentences_unsupported_lang_returns_bad_request()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            
            Should.Throw<ArgumentException>(() => controller.GetSentences("xx"));
        }

        [Fact]
        public void Translate_valid_returns_translated_text()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var request = new EmergencyPhrasebookTranslateRequestDto
            {
                Key = "POL_HELP",
                SourceLang = "en",
                TargetLang = "sr"
            };

            var result = controller.Translate(request).ShouldBeOfType<OkObjectResult>();
            var dto = result.Value.ShouldBeOfType<EmergencyPhrasebookTranslateResponseDto>();

            dto.Key.ShouldBe("POL_HELP");
            dto.SourceLang.ShouldBe("en");
            dto.TargetLang.ShouldBe("sr");
            dto.SourceText.ShouldNotBeNullOrWhiteSpace();
            dto.TranslatedText.ShouldNotBeNullOrWhiteSpace();
            dto.TranslatedText.ShouldBe("Upomoć!");
        }

        [Fact]
        public void Translate_unknown_key_returns_bad_request_with_message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateControllerWithRole(scope, "-21", "tourist");

            var request = new EmergencyPhrasebookTranslateRequestDto
            {
                Key = "NOPE",
                SourceLang = "en",
                TargetLang = "sr"
            };

            var result = controller.Translate(request).ShouldBeOfType<BadRequestObjectResult>();

            
            result.Value.ShouldNotBeNull();
        }

        private static EmergencyPhrasebookController CreateControllerWithRole(IServiceScope scope, string userId, string role)
        {
            return new EmergencyPhrasebookController(
                scope.ServiceProvider.GetRequiredService<IEmergencyPhrasebookService>())
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }

        private static ControllerContext BuildContextWithRole(string id, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
