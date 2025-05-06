using DataAccessLayer.Repositories;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Diagnostics;

namespace BusinessLayer.Concrete
{
    public class AccountManager : IAccountService
    {
        private readonly AccountRepository _accountRepository;
        
        // Constructor
        public AccountManager()
        {
            _accountRepository = new AccountRepository();
        }
        // İsim soyisimden müşteri bilgilerine ulaşmak
       
        
        // Verilen AccountNumber'a göre veritabanından bir account getirir.
        public Task<Account> GetByAccountNumber(string accountNumber)
            {
                
                var account = _accountRepository.GetByAccountNumber(accountNumber);
                // Verilen AccountNumber'a göre eşleşen ilk hesabı buluyoruz
                
            if (account == null)
            {
                return null;
            }
                    return Task.FromResult(account);
            }
        // Hesap listeleme
        public List<Account> GetAllAccount()
        {
            // Veritabanındaki account verilerini alıyoruz
            var accounts = _accountRepository.GetAll();
            return accounts;
        }
        // Bakiye kontrolü
        public Task<decimal> CheckBalance(string accountNumber)
        {
            var bankAccount = _accountRepository.GetByAccountNumber(accountNumber);
            if (bankAccount == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            return Task.FromResult(bankAccount.Balance);
        }
        // Hesap pasifleştirme
        public Task<bool> PassiveAccount(string accountNumber)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            if (!account.IsActive) // Hesap zaten pasifse
            {
                throw new Exception("Hesap zaten pasif.");
            }
            account.IsActive = false; // Hesabı pasif yapıyoruz
            _accountRepository.Update(account); // Veritabanında hesap bilgilerin güncelle.
            return Task.FromResult(true); 
        }
        // Hesap aktifleştirme
        public Task<bool> ActiveAccount(string accountNumber)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            if (account.IsActive)
            {
                throw new Exception("Hesap zaten aktif");
            }
            account.IsActive = true;
            _accountRepository.Update(account);

            return Task.FromResult(true);
        }
        // Para Yatırma
          public Task<bool> Deposit(string accountNumber, decimal amount)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);
                if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            if (!account.IsActive) {
                throw new Exception("Hesap aktif değil.");
            }
            if (amount <= 0)
            {
                throw new Exception("Yatırılacak tutar pozitif olmalı.");
            }
            account.Balance += amount;
            _accountRepository.Update(account);

            return Task.FromResult(true);
        }
        // Para Çekme
        public Task<bool> Withdraw(string accountNumber, decimal amount)
        {
            var account = _accountRepository.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            if (!account.IsActive)
            {
                throw new Exception("Hesap aktif değil.");
            }
            if (amount <= 0)
            {
                throw new Exception("Çekilecek tutar pozitif olmalı.");
            }
            account.Balance -= amount;
            _accountRepository.Update(account);
            return Task.FromResult(true);
        }
        // Para Transferi
        public Task<bool> Transfer(string senderAccountNumber, string recevierAccountNumber, decimal amount)
        {
            var senderaccount = _accountRepository.GetByAccountNumber(senderAccountNumber);
            var recevieraccount = _accountRepository.GetByAccountNumber(recevierAccountNumber);
            if (senderaccount == null)
            {
                throw new Exception("Banka hesabı bulunamadı");
            }
            if (!senderaccount.IsActive)
            {
                throw new Exception("Hesap aktif değil");
            }
           
            if (recevieraccount == null)
            {
                throw new Exception("Böyle bir banka hesabı bulunamadı.");
            }
            if (!recevieraccount.IsActive)
            {
                throw new Exception("Hesap aktif değil");
            }
            if (amount <= 0)
            {
                throw new Exception("Yatırılacak tutar pozitif olmalı.");
            }
            if (senderaccount.Balance < amount)
            {
                throw new Exception("Bakiye yetersiz");
            }
            senderaccount.Balance -= amount;
            recevieraccount.Balance += amount;
            _accountRepository.Update(senderaccount);
            _accountRepository.Update(recevieraccount);
            return Task.FromResult(true);
        }

     
    }
}
