namespace Explorer.Stakeholders.API.Dtos.Emergency
{
    public class EmergencyTranslationResponseDto
    {
        public string TranslatedText { get; set; } = "";
        public bool IsFallback { get; set; }
    }
}
