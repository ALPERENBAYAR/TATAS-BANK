using BusinessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLayer.Concrete
{
    public class ManagerManager : IManagerService
    {
        private readonly AccountRepository _accountRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly BranchRepository _branchRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly BankRepository _bankRepository;
        private readonly ManagerRepository _managerRepository;
        public ManagerManager()
        {
            _accountRepository = new AccountRepository();
            _employeeRepository = new EmployeeRepository();
            _branchRepository = new BranchRepository();
            _customerRepository = new CustomerRepository();
            _bankRepository = new BankRepository();
            _managerRepository = new ManagerRepository();
        }
        public string GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999).ToString();
            
        }
        public async Task<Account> CreateAccount(int customerId, decimal balance, string ignoredAccountNumber, int branchId, int bankId)
        {
            // 1) Müşteriyi yükle
            var customer = _customerRepository.GetById(customerId)
                          ?? throw new Exception("Müşteri bulunamadı.");

            // 2) Şubeyi ve bankayı yükle
            var branch = _branchRepository.GetById(branchId)
                         ?? throw new Exception("Şube bulunamadı.");
            var bank = _bankRepository.GetById(bankId)
                       ?? throw new Exception("Banka bulunamadı.");

            // 3) Hesap numarasını random oluştur
            string accountNumber;
            do
            {
                accountNumber = GenerateAccountNumber();
            }
            // Aynı numara var mı kontrol et
            while (_accountRepository.GetAll().Any(a => a.AccountNumber == accountNumber));

            // 4) Yeni Account entity’si oluştur
            var newAccount = new Account
            {
                Customer = customer,
                AccountNumber = accountNumber,
                Balance = balance,
                IsActive = true,
                BranchId = branchId,
                BankId = bankId
            };

            // 5) Veritabanına ekle
            _accountRepository.Insert(newAccount);
            return newAccount;
        }
        public Task<Employee> AddEmployee(string employeeName, string employeeSurname, string employeeMail, string employeePassword, int branchId)
        {
            var existingEmployee = _employeeRepository.GetAll().FirstOrDefault(e => e.EmployeeMail == employeeMail);
            if (existingEmployee != null)
            {
                throw new Exception("Bu e-posta ile açılmış bir çalışan zaten var.");
            }
            var hashedPassword = PasswordHasher.HashPassword(employeePassword);
            var employee = new Employee
            {
                EmployeeName = employeeName,
                EmployeeSurname = employeeSurname,
                EmployeeMail = employeeMail,
                EmployeePassword = hashedPassword,
                BranchId = branchId
            };
           
            _employeeRepository.Insert(employee);
            return Task.FromResult(employee);

        }
        public Task<Employee> DeleteEmployee(string employeeMail)
        {
            var existingEmployee = _employeeRepository.GetAll().FirstOrDefault(c => c.EmployeeMail == employeeMail);
            if (existingEmployee == null)
            {
                throw new Exception("Bu e-posta bir çalışana ait değil.");
            }
             _employeeRepository.Delete(existingEmployee);
            return Task.FromResult(existingEmployee);
        }
        public async Task<bool> DeActiveAccount(string AccountNumber)
        {
            
            var accounts = _accountRepository.GetAll();
            var account = accounts.FirstOrDefault(c => c.AccountNumber == AccountNumber);



            if (account == null)
            {
                throw new Exception("Hesap bulunamadı");
            }
            if (!account.IsActive)
            {
                return false; // Hesap zaten devre dışı işlem yapmıyoruz
            }
            account.IsActive = false; // Hesap aktifse devre dışı yapıyoruz

            _accountRepository.Update(account); // Hesap bilgilerini veritabanında güncelliyoruz
            return true;
        }
        public async Task<bool> UpdateApp(Customer customer, int AccountNumber)
        {
            // Hesap numarasına göre ilgili hesabı alıyoruz
            var account = _accountRepository.GetById(AccountNumber);
            // Hesap bulunmazsa hata fırlatıyoruz
            if (account == null)
            {
                throw new Exception("Hesap bulunamadı");
            }
            // Hesap bilgilerini güncelliyoruz
            account.Customer = customer;

            // Hesap veritabanında güncelleme
            _accountRepository.Update(account);
            return true;
        }
        public Task<Branch> AddBranch(string branchName, string branchCity)
        {
            var existingBranch = _branchRepository.GetAll().FirstOrDefault(c => c.BranchName == branchName && c.BranchCity == branchCity);
           if (existingBranch != null)
            {
                throw new Exception("Böyle bir şube zaten bulunamkta.");
            }
            var branch = new Branch
            {
                BranchName = branchName,
                BranchCity = branchCity
            };
            _branchRepository.Insert(branch);
            return Task.FromResult(branch);

            }
        public Task<List<Employee>> ListEmployee()
        {
            var employees = _employeeRepository.GetAll().ToList();
            return Task.FromResult(employees);
        }
        public async Task<bool> ChangePassword(string managerMail, string oldPassword, string newPassword)
        {
            using (var context = new Context())
            {
                var manager = context.Managers.FirstOrDefault(m => m.ManagerMail == managerMail);
                if (manager == null)
                {
                    throw new Exception("Yönetici bulunamadı.");
                }

                var hashedOldPassword = PasswordHasher.HashPassword(oldPassword);

                if (manager.ManagerPassword != hashedOldPassword)
                {
                    return false; // Şu anki şifre yanlış
                }

                manager.ManagerPassword = newPassword;
                _managerRepository.Update(manager);
                return true;

            }
        }


    }
}
