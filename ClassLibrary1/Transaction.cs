using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionFee { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
        public Account Account { get; set; }
    }
}
