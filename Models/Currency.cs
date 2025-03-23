namespace exchangeRateApi.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string? country_Code { get; set; }
        public int Country_number { get; set; }
        public string? Country { get; set; }
        public string? Currency_Name { get; set; }
        public string? Currency_Code { get; set; }
        public int Currency_Number { get; set; }
        public float Value { get; set; }
    }
}
