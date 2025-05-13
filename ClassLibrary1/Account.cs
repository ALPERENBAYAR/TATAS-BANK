using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        
        public Customer Customer { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int BankId { get; set; }
        public Bank Bank { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
        public int CustomerId { get; set; }

    }
}
