using DataAccessLayer.Concrete;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public Customer CustomerNameAndSurname(string customerName, string customerSurname)
        { 
            using var c = new Context();
            return c.Set<Customer>().FirstOrDefault(c => c.CustomerName == customerName && c.CustomerSurname == customerSurname);
            
        }
       /* public Customer GetMailAndPass(string customerMail, string customerPassword)
        {
            using var c = new Context();
            return c.Set<Customer>().FirstOrDefault(c => c.CustomerMail == customerMail && c.CustomerPassword == customerPassword);
        }*/
    }
}
