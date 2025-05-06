using BusinessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{

    public class TransactionManager : ITransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        public TransactionManager()
        {
            _transactionRepository = new TransactionRepository();
        }
        // Para yatırma kaydı
        public void DepositLog(string accountNumber, decimal amount)
        {
            var transaction = new Transaction
            {
                SenderAccountNumber = accountNumber,
                ReceiverAccountNumber = null,
                Amount = amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Para Yatırma",
            };
            _transactionRepository.Insert(transaction);


        }
        // Para transfer kaydı
        public void Transfer(string senderAccountNumber, string receiverAccountNumber, decimal amount)
        {
            var senderAccount = _transactionRepository.GetByAccountNumber(senderAccountNumber);
            var receiverAccount = _transactionRepository.GetByAccountNumber(receiverAccountNumber);
            if (senderAccount == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            if (receiverAccount == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            var transaction = new Transaction
            {
                SenderAccountNumber = senderAccountNumber,
                ReceiverAccountNumber = receiverAccountNumber,
                Amount = amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Transfer",
            };
            _transactionRepository.Insert(transaction);
        }
            // Para çekme kaydı
        public void WithdrawLog(string accountNumber, decimal amount)
        {
            var account = _transactionRepository.GetByAccountNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("Banka hesabı bulunamadı.");
            }
            var transaction = new Transaction
            {
                SenderAccountNumber = accountNumber,
                ReceiverAccountNumber = null,
                Amount = amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Para çekme",
            };
             _transactionRepository.Insert(transaction);
        }
        // İşlem geçmişi listeleme
        // Paging yapılmalı
        public List<Transaction> GetTransactionsByAccount(string accountNumber)
        {
            var all = _transactionRepository.GetAll();

            var history = all
                // .Where ile filtreleme işlemi yapılır.
                .Where(t => t.SenderAccountNumber == accountNumber || t.ReceiverAccountNumber == accountNumber)
                // OrderByDescending ile en güncelden geçmiş tarihe göre sıralanır
                .OrderByDescending(t => t.TransactionDate)
                // Listeleme
                .ToList();

            return history;
        }

        // Hata Yönetimi yapılabilir


    }
}
