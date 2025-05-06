// Bankacilik.Models/AccountViewModel.cs
using EntityLayer;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Bankacilik.Models
{
    public class AccountViewModel
    {
        // Formdan gelen alanlar
        public string CustomerMail { get; set; }
        public int SelectedBranchId { get; set; }
        public int SelectedBankId { get; set; }
        public decimal Balance { get; set; }

        // Controller’da doldurduğumuz, görüntüleme için gereken alanlar
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string BankName { get; set; }

        // Mevcut hesapları tutan liste
        public List<AccountViewModel> Accounts { get; set; } = new List<AccountViewModel>();
        public List<CustomerViewModel> Customers { get; set; }

        // Dropdown listelerini doldurduğumuz koleksiyonlar
        public List<Branch> Branches { get; set; } = new();
        public List<Bank> Banks { get; set; } = new();
    }
}
