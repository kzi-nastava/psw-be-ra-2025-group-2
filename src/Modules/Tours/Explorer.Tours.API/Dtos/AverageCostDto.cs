namespace Explorer.Tours.API.Dtos
{
    public class AverageCostDto
    {
        public decimal TotalPerPerson { get; set; }

        
        public string Currency { get; set; } = "RSD";

        public AverageCostBreakdownDto Breakdown { get; set; } = new();


        public string Disclaimer { get; set; } =
            "Real expenses can vary from those which are shown, they are just informative.";
    }
}
