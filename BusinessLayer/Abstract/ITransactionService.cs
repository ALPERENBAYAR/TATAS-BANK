using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface ITransactionService
    {
            void DepositLog(string accountNumber, decimal amount);
            void WithdrawLog(string accountNumber, decimal amount);
            void Transfer(string senderAccountNumber, string receiverAccountNumber, decimal amount);
     
        }
    }


