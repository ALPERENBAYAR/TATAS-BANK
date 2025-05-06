using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IEmployeeService
    {
        Task<Account> CreateAccount(Customer customer, decimal Balance, string AccountNumber);
        Task<bool> TransactionFee(decimal TransactionFee, string AccountNumber);
        Task<bool> UpdateApp(Customer customer, int AccountNumber);
      

    }
}
