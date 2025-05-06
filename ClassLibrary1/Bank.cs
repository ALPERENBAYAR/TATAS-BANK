using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Bank
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public List<Branch> Branches { get; set; } = new();
        public List<Account> Accounts { get; set; } = new();
    }
}
