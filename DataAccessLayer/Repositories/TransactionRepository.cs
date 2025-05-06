using DataAccessLayer.Concrete;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Repositories
{
    public class TransactionRepository : GenericRepository<EntityLayer.Transaction>
    {
        public EntityLayer.Transaction GetByAccountNumber(string accountNumber)
        {
            using var c = new Context();
            return c.Set<EntityLayer.Transaction>().FirstOrDefault(c => c.AccountNumber == accountNumber);
        }

    }
}
