namespace CurrencyConvert.Models
{
    public class Currency
    {
        public string Code { set; get;  } //primary key
        public int OrderNum { set; get; } = 1;
        public string Name { set; get; }
        public string NameLatin { set; get; }
    }
    
}