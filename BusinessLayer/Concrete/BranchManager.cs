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
    public class BranchManager : IBranchService
    {
        private readonly GenericRepository<Branch> _branchRepository;
        private readonly GenericRepository<Account> _accountRepository;
        public BranchManager()
        {
            _branchRepository = new GenericRepository<Branch>();
            _accountRepository = new GenericRepository<Account>();
        }
        public async Task<Branch> AddBranch(string BranchName)
        {
            if (string.IsNullOrWhiteSpace(BranchName))
            {
                throw new ArgumentNullException("Şube adı boş olamaz.");
            }
            // Yeni şube oluşturuluyor
            var newBranch = new Branch
            {
                BranchName = BranchName,

            };

            // Şube veritabanına kaydediliyor
             _branchRepository.Insert(newBranch);
            return newBranch;
        }

        public Task<bool> DeActiveBranch(string BranchName)
        {
            throw new NotImplementedException();
        }
    }
}
