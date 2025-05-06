using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public interface IAccountService
    {
        Task<Account> GetByAccountNumber(string AccountNumber);
        List<Account> GetAllAccount();
        Task<bool> Deposit(string accountNumber, decimal amount);
        Task<bool> Withdraw(string accountNumber, decimal amount);
        Task<bool> PassiveAccount(string accountNumber);
        Task<bool> ActiveAccount(string accountNumber);


    } 
}
