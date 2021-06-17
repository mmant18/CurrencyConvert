namespace CurrencyConvert.Models
{
    public class Record
    {
        public int Id { set; get; } = 1; //primary key
        public string CurrencyIn { set; get; }
        public string CurrencyOut { set; get; }
        public double AmountIn { set; get; }
        public double AmountOut { set; get; }
        public System.DateTime Date { set; get; }
        public string Comment { set; get; }

        public double AmountInLari { set; get; }
    }
}