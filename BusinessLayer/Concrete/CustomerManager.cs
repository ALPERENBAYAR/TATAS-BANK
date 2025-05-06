using BusinessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly AccountRepository _accountRepository;
        public CustomerManager()
        {
            _customerRepository = new CustomerRepository();
            _accountRepository = new AccountRepository();
        }
     
        // CUSTOMER HESABINA GİRİŞ METODU
        public  Task<Customer> Login(string CustomerMail, string CustomerPassword)
        {
            // Tüm customer verilerini alıyoruz
            var customers = _customerRepository.GetAll();

            //Girilen şifre ve mail ile eşleşen müşteri var mı kontrol ediyoruz
            var customer = customers.FirstOrDefault(c => c.CustomerMail == CustomerMail && c.CustomerPassword == CustomerPassword);
            return Task.FromResult(customer);

        }

        // CUSTOMER ŞİFRE GÜNCELLEME METODU
        public async Task<bool> ChangePassword(string customerMail, string oldPassword, string newPassword)
        { // Müşteriyi veritabanında bul

            var customer = _customerRepository.GetAll().FirstOrDefault(c => c.CustomerMail == customerMail);
            if (customer == null)
            {
                throw new Exception("Müşteri bulunamadı.");
            }
            var hashedOldPassword = PasswordHasher.HashPassword(oldPassword);

            // Veritabanındaki hash ile karşılaştır
            if (customer.CustomerPassword != hashedOldPassword)
            {
                return false; 
            }
            // Şifreyi değiştir
            customer.CustomerPassword = newPassword;
            _customerRepository.Update(customer); //Customerı veritabanında güncelle

            return true;
        
        }

        // CUSTOMER AD VE SOYAD İLE ÇAĞIRMA METODU
        public Customer CustomerNameAndSurname(string customerName, string customerSurname)
        {
            // Veritabanındaki customer verilerini alıyoruz
            var customer = _customerRepository.CustomerNameAndSurname(customerName, customerSurname);
            // Customer lsitesinde ad soyada göre yapılır varsa müşteri yoksa null döndürülür.

            if (customer == null)
            {
                return null;
            }
            return customer;
        }

      //   GEREK YOK AMA ŞİMDİLİK KALSIN
       public Customer GetById(int Id)
        {
            var customer = _customerRepository.GetById(Id);
            if (customer == null)
            {
                return null;
            }
            return customer;
        }

        public async Task<bool> TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (fromAccountNumber == toAccountNumber)
                throw new Exception("Kendi hesabınıza para transfer edemezsiniz.");

            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);
            var toAccount = _accountRepository.GetByAccountNumber(toAccountNumber);

            if (fromAccount == null || toAccount == null)
                throw new Exception("Gönderici veya alıcı hesap bulunamadı.");

            decimal transactionFee = amount * 0.01m;
            decimal totalAmount = amount + transactionFee;

            if (fromAccount.Balance < totalAmount)
                throw new Exception($"Yetersiz bakiye. İşlem ücreti dahil: {totalAmount} TL");

            fromAccount.Balance -= totalAmount;
            toAccount.Balance += amount;

            _accountRepository.Update(fromAccount);
            _accountRepository.Update(toAccount);

            return true;
        }

        // EFT METODU (müşteri -> sistem dışı)
        public async Task<bool> TransferEft(string fromAccountNumber, decimal amount)
        {
            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);

            if (fromAccount == null)
                throw new Exception("Gönderen hesap bulunamadı.");

            decimal fee = amount * 0.01m;
            decimal total = amount + fee;

            if (fromAccount.Balance < total)
                throw new Exception($"Yetersiz bakiye. İşlem ücreti dahil: {total:F2} TL");

            fromAccount.Balance -= total;
            _accountRepository.Update(fromAccount);

            return true;
        }

        }
    }
