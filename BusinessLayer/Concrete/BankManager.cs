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
    public class BankManager : IBankService
    {
        private readonly BranchRepository _branchRepository;
        private readonly AccountRepository _accountRepository;
     /*   public List<Branch> GetBranchesForBank(int bankId)
        {
            var branches = _branchRepository
            .GetAll()
            .Where(b => b.BankId == bankId)
            .ToList();
            return branches;
        }*/
        public List<Account> GetAccountsForBank(int bankId)
        {
            var accounts = _accountRepository
                .GetAll()
                .Where(b => b.BankId == bankId)
                .ToList();
            return accounts;
        }
    }
}
