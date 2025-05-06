using DataAccessLayer.Concrete;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>
    {
        public Employee GetByAccountNumber(string accountNumber)
        {
            using var c = new Context();
            return c.Set<Employee>().FirstOrDefault(c => c.AccountNumber == accountNumber);
        }
    }
}
