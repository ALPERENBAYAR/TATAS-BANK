using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string EmployeePassword { get; set; }
        public string EmployeeMail { get; set; }
        public int? AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public Account Account { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

    }
}
