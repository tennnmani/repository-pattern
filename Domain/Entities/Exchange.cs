namespace Domain.Entities
{
    public class Exchange
    {
        public DateTime date { get; set; }
        public string iso3 { get; set; }
        public string name { get; set; }
        public decimal buy { get; set; }
        public decimal sell { get; set; }
        public int unit { get; set; }
    }
}
