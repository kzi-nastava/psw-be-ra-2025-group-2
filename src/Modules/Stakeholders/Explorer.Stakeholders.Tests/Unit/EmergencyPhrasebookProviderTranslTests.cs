using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using Explorer.Stakeholders.Infrastructure.Translation.Emergency;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class EmergencyPhrasebookProviderTranslTests
    {
        [Fact]
        public void Provider_loads_languages_and_phrases_from_disk()
        {
            var root = Path.Combine(Path.GetTempPath(), "emergency_test_" + Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(root, "Translation", "Emergency", "Resources"));

            File.WriteAllText(Path.Combine(root, "Translation", "Emergency", "Resources", "languages.json"),
                """
                [
                  { "code": "en", "name": "English" },
                  { "code": "sr", "name": "Srpski" }
                ]
                """);

            File.WriteAllText(Path.Combine(root, "Translation", "Emergency", "Resources", "phrasebook.json"),
                """
                {
                  "POL_HELP": { "en": "Help!", "sr": "Upomoć!" }
                }
                """);

            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.ContentRootPath).Returns(root);

            var logger = new Mock<ILogger<EmergencyPhrasebookProviderTransl>>();

            var sut = new EmergencyPhrasebookProviderTransl(env.Object, logger.Object);

            var langs = sut.GetLanguages();
            langs.Count.ShouldBe(2);

            var phrase = sut.TryGetPhrase("POL_HELP");
            phrase.ShouldNotBeNull();
            phrase!["sr"].ShouldBe("Upomoć!");

            Directory.Delete(root, recursive: true);
        }
    }
}
