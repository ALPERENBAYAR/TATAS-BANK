using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
   
     public class Customer
    {

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerPassword { get; set; }
        public string CustomerMail { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public List<Account> Accounts { get; set; } = new();

    }
}
