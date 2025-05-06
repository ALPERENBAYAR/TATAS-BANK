namespace Bankacilik.Models
{
    public class TransferViewModel
    {
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal Amount { get; set; }

        // Yeni Alanlar
        public decimal TransactionFee { get; set; } // Sabit işlem ücreti
        public decimal Fee  => Amount * 0.01m;

        public decimal TotalAmount => Amount + TransactionFee; // Gönderici toplamda ne kadar öder
    }
}