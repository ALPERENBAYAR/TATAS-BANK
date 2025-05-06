using EntityLayer;
using System.Collections.Generic;

namespace Bankacilik.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string EmployeeMail { get; set; }
        public string EmployeePassword { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public List<Branch> Branches { get; set; } = new List<Branch>();
        public List<EmployeeViewModel> EmployeeList { get; set; } = new List<EmployeeViewModel>();
    }


    }

