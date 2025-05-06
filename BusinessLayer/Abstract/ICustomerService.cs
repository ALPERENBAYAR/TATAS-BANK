using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface ICustomerService
    {
        Task<Customer> Login(string CustomerMail, string CustomerPassword);
  
    }
}
