using System.Collections.Generic;
using EntityLayer;
namespace Bankacilik.Models
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerMail { get; set; }
        public string CustomerPassword { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public List<CustomerViewModel> customers { get; set; }
        public List<Branch> branches { get; set; }
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }
}
