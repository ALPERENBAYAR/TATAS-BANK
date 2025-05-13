namespace Bankacilik.Models
{
    public class Kredi
    {
        public int Id { get; set; }
        public string MusteriAdi { get; set; }
        public decimal Tutar { get; set; }
        public int Vade { get; set; } // Ay cinsinden
        public double FaizOrani { get; set; }
    }
}
