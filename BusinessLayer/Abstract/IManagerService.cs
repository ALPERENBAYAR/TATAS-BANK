using EntityLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IManagerService
    {
       
        Task<bool> DeActiveAccount(string AccountNumber);
        Task<bool> UpdateApp(Customer customer,int AccountNumber);
        
        
    }
}
