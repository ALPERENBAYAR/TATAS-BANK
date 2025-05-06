using DataAccessLayer.Concrete;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository : GenericRepository<Account> 
    {
        public Account GetByAccountNumber(string accountNumber)
        {
            using var c = new Context();
            return c.Set<Account>().FirstOrDefault(c => c.AccountNumber == accountNumber);
        }
    }
}
