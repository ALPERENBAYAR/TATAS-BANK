using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Abstract
{
    public interface ITransactionDal : IGenericDal<Transaction>
    {
    }
}
