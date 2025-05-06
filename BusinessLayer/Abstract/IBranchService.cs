using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBranchService
    {
      Task<Branch> AddBranch(string BranchName);
        Task<bool> DeActiveBranch(string BranchName);
    }
}
