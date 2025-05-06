using BusinessLayer.Abstract;
using EntityLayer;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Concrete;

namespace BusinessLayer.Concrete
{
    public class EmployeeManager : IEmployeeService
    {
        private readonly AccountRepository _accountRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly CustomerRepository _customerRepository;
        public EmployeeManager()
        {
            _accountRepository = new AccountRepository();
            _employeeRepository = new EmployeeRepository();
            _customerRepository = new CustomerRepository();
        }
        public string GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999).ToString();
        }
        // Hesap oluşturma
        public async Task<Account> CreateAccount(Customer customer, decimal Balance, string AccountNumber)
        {
            string accountNumber = GenerateAccountNumber();
            var newAccount = new Account
            {
                Customer = customer,


                AccountNumber = accountNumber,
                Balance = 0 // Başlangıçta bakiye 0
            };
           _accountRepository.Insert(newAccount);
            return newAccount;
        }
      
        public async Task<bool> UpdateApp(Customer customer, int AccountNumber)
        {
            // Hesap numarasına göre ilgili hesabı alıyoruz
            var account =  _accountRepository.GetById(AccountNumber);
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
        // Hesap Silme
        public Task<bool> DeleteAccount(string accountNumber)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            _accountRepository.Delete(account);
            return Task.FromResult(true);
        }
        public Task<Customer> AddCustomer(string CustomerName, string CustomerSurname, string customerMail, string CustomerPassword)
        {
            var existingCustomer = _customerRepository.GetAll().FirstOrDefault(c => c.CustomerMail.ToLower() == customerMail.ToLower());
            if (existingCustomer != null)
            {
                throw new Exception("Bu e-postaya ait bir müşteri zaten var.");
            }
            var hashedPassword = PasswordHasher.HashPassword(CustomerPassword);
            var customer = new Customer()
            {
                CustomerName = CustomerName,
                CustomerSurname = CustomerSurname,
                CustomerMail = customerMail,
                CustomerPassword = hashedPassword,
            };
            _customerRepository.Insert(customer);
            return Task.FromResult(customer);
        }
        public Task<Customer> DeleteCustomer(string CustomerMail)
        {
            var existingCustomer = _customerRepository.GetAll().FirstOrDefault(c => c.CustomerMail == CustomerMail);
            if (existingCustomer == null)
            {
                throw new Exception("Böyle bir e-postaya ait bir hesap zaten yok.");
            }
            _customerRepository.Delete(existingCustomer);
            return Task.FromResult(existingCustomer);
        }
        public async Task<Employee> Login(string EmployeeMail, string EmployeePassword)
        {
            var hashedPassword = PasswordHasher.HashPassword(EmployeePassword);
            // Tüm customer verilerini alıyoruz
            var employees = _employeeRepository.GetAll();

            //Girilen şifre ve mail ile eşleşen müşteri var mı kontrol ediyoruz
            var employee = employees.FirstOrDefault(c => c.EmployeeMail == EmployeeMail && c.EmployeePassword == hashedPassword);
            return employee;

        }

        public async Task<bool> TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (fromAccountNumber == toAccountNumber)
                throw new Exception("Kendi hesabınıza para transfer edemezsiniz.");

            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);
            var toAccount = _accountRepository.GetByAccountNumber(toAccountNumber);

            if (fromAccount == null || toAccount == null)
                throw new Exception("Gönderici veya alıcı hesap bulunamadı.");

            decimal transactionFee = 0m; // Sabit işlem ücreti yok
            decimal totalAmount = amount + transactionFee;

            if (fromAccount.Balance < totalAmount)
                throw new Exception("Yetersiz bakiye. İşlem ücreti dahil: " + totalAmount);

            fromAccount.Balance -= totalAmount;
            toAccount.Balance += amount;

            _accountRepository.Update(fromAccount);
            _accountRepository.Update(toAccount);

            return true;
        }

        public async Task<bool> TransactionFee(decimal transactionFee, string accountNumber)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);

            if (account == null || account.Balance < transactionFee)
            {
                return false;
            }
            account.Balance -= transactionFee;
            _accountRepository.Update(account);
            return true;
        }
        public async Task<bool> WithdrawMoney(string fromAccountNumber, decimal amount, decimal transactionFee)
        {
            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);
            if (fromAccount == null)
                throw new Exception("Hesap bulunamadı.");

            decimal total = amount + transactionFee;

            if (fromAccount.Balance < total)
                throw new Exception("Yetersiz bakiye.Bakiyeniz");

            fromAccount.Balance -= total;
            _accountRepository.Update(fromAccount);

            return true;
        }
        public async Task<bool> FastTransfer(string fromAccountNumber, decimal amount, decimal transactionFee)
        {
            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);
            if (fromAccount == null)
                throw new Exception("Gönderen hesap bulunamadı.");

            decimal total = amount + transactionFee;

            if (fromAccount.Balance < total)
                throw new Exception($"Yetersiz bakiye. Bakiyeniz: {fromAccount.Balance} ₺, Gerekli Tutar: {total} ₺");

            fromAccount.Balance -= total;
            _accountRepository.Update(fromAccount);

            return true;
        }
        public async Task<bool> DepositMoney(string toAccountNumber, decimal amount)
        {
            var toAccount = _accountRepository.GetByAccountNumber(toAccountNumber);
            if (toAccount == null)
                throw new Exception("Hesap bulunamadı.");

            toAccount.Balance += amount;
            _accountRepository.Update(toAccount);

            return true;
        }
        public async Task<bool> ChangePassword(string employeeMail, string oldPassword, string newPassword)
        {
            using (var context = new Context())
            {

                var employee = context.Employees.FirstOrDefault(e => e.EmployeeMail == employeeMail);
                if (employee == null)
                {
                    throw new Exception("Banka memuru bulunamadı.");
                }
                var hashedOldPassword = PasswordHasher.HashPassword(oldPassword);
                if (employee.EmployeePassword != hashedOldPassword)
                {
                    return false; // Mevcut şifre yanlış
                }

                employee.EmployeePassword = newPassword;
                _employeeRepository.Update(employee);
                return true;
            }
        }
    }
}
