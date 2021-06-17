namespace CurrencyConvert.Models
{
    public class ExchangeRate
    {
        public int Id { set; get; } = 1; //primary key
        public string CurrencyCode { set; get; }
        public double BuyRate { set; get; }
        public double SellRate { set; get; }
    }
}