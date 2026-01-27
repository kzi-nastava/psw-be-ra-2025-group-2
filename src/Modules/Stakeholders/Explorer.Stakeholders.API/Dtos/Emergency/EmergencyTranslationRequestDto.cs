namespace Explorer.Stakeholders.API.Dtos.Emergency
{
    public class EmergencyTranslationRequestDto
    {
        public string CountryCode { get; set; } = "";
        public string SourceLanguage { get; set; } = "";
        public string TargetLanguage { get; set; } = "";
        public string Text { get; set; } = "";
    }
}
